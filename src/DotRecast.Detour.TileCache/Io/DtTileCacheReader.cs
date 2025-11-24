/*
Copyright (c) 2009-2010 Mikko Mononen memon@inside.org
recast4j copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org
DotRecast Copyright (c) 2023-2024 Choi Ikpil ikpil@naver.com

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
 claim that you wrote the original software. If you use this software
 in a product, an acknowledgment in the product documentation would be
 appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
 misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

using System.IO;
using DotRecast.Core;
using DotRecast.Detour.Io;

namespace DotRecast.Detour.TileCache.Io
{
    public readonly struct DtTileCacheReader
    {
        private readonly IRcCompressor _compressor;

        public DtTileCacheReader(IRcCompressor compressor)
        {
            _compressor = compressor;
        }

        public DtTileCache Read(BinaryReader bb, int maxVertPerPoly, IDtTileCacheMeshProcess meshProcessor)
        {
            var header = ReadHeader(bb);
            var tc = Create(header, maxVertPerPoly, meshProcessor);
            ReadTiles(bb, tc, header);
            return tc;
        }

        public DtTileCache Create(DtTileCacheSetHeader header, int maxVertPerPoly, IDtTileCacheMeshProcess meshProcessor)
        {
            DtNavMesh mesh = new();
            mesh.Init(header.meshParams, maxVertPerPoly);
            DtTileCache tc = new(header.cacheParams, mesh, _compressor, meshProcessor);
            return tc;
        }

        public DtTileCacheSetHeader ReadHeader(BinaryReader bb)
        {
            DtTileCacheSetHeader header = default;
            header.magic = bb.ReadInt32();
            if (header.magic != DtTileCacheSetHeader.TILECACHESET_MAGIC)
            {
                if (header.magic != DtTileCacheSetHeader.TILECACHESET_MAGIC)
                {
                    throw new IOException("Invalid magic");
                }
            }

            header.version = bb.ReadInt32();
            if (header.version != DtTileCacheSetHeader.TILECACHESET_VERSION)
            {
                throw new IOException("Invalid version");
            }

            header.numTiles = bb.ReadInt32();
            DtNavMeshParamsReader paramReader;
            header.meshParams = paramReader.Read(bb);
            header.cacheParams = ReadCacheParams(bb);

            return header;
        }

        public void ReadTiles(BinaryReader bb, DtTileCache tc, DtTileCacheSetHeader header)
        {
            // Read tiles.
            for (int i = 0; i < header.numTiles; ++i)
            {
                ReadTile(bb, tc);
            }
        }

        public void ReadTile(BinaryReader bb, DtTileCache tc)
        {
            long tileRef = bb.ReadInt32();
            int dataSize = bb.ReadInt32();
            if (tileRef == 0 || dataSize == 0)
                return;

            byte[] data = bb.ReadBytes(dataSize);
            long tile = tc.AddTile(data, 0);
            if (tile != 0)
            {
                tc.BuildNavMeshTile(tile);
            }
        }

        public void ReadTile(BinaryReader bb, DtTileCache tc, int tx, int ty)
        {
            long tileRef = bb.ReadInt32();
            int dataSize = bb.ReadInt32();
            if (tileRef == 0 || dataSize == 0)
                return;

            byte[] data = bb.ReadBytes(dataSize);
            data = tc.AddTilePosition(data, tx, ty);
            long tile = tc.AddTile(data, 0);
            if (tile != 0)
            {
                tc.BuildNavMeshTile(tile);
            }
        }

        private DtTileCacheParams ReadCacheParams(BinaryReader bb)
        {
            DtTileCacheParams option = new();

            option.orig.X = bb.ReadSingle();
            option.orig.Y = bb.ReadSingle();
            option.orig.Z = bb.ReadSingle();

            option.cs = bb.ReadSingle();
            option.ch = bb.ReadSingle();
            option.width = bb.ReadInt32();
            option.height = bb.ReadInt32();
            option.walkableHeight = bb.ReadSingle();
            option.walkableRadius = bb.ReadSingle();
            option.walkableClimb = bb.ReadSingle();
            option.maxSimplificationError = bb.ReadSingle();
            option.maxTiles = bb.ReadInt32();
            option.maxObstacles = bb.ReadInt32();
            return option;
        }
    }
}