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
using DotRecast.Detour.TileCache.Io.Compress;

namespace DotRecast.Detour.TileCache.Io
{
    public struct DtTileCacheWriter
    {
        private readonly DtNavMeshParamWriter paramWriter = new DtNavMeshParamWriter();
        private readonly IDtTileCacheCompressorFactory _compFactory;

        public DtTileCacheWriter(IDtTileCacheCompressorFactory compFactory)
        {
            _compFactory = compFactory;
        }


        public void Write(BinaryWriter stream, DtTileCache cache)
        {
            RcIO.Write(stream, DtTileCacheSetHeader.TILECACHESET_MAGIC);
            RcIO.Write(stream, DtTileCacheSetHeader.TILECACHESET_VERSION);
            int numTiles = 0;
            for (int i = 0; i < cache.GetTileCount(); ++i)
            {
                DtCompressedTile tile = cache.GetTile(i);
                if (tile == null || tile.data == null)
                    continue;
                numTiles++;
            }

            RcIO.Write(stream, numTiles);
            paramWriter.Write(stream, cache.GetNavMesh().GetParams());
            WriteCacheParams(stream, cache.GetParams());
            for (int i = 0; i < cache.GetTileCount(); i++)
            {
                DtCompressedTile tile = cache.GetTile(i);
                if (tile == null || tile.data == null)
                    continue;
                RcIO.Write(stream, (int)cache.GetTileRef(tile));
                byte[] data = tile.data;
                DtTileCacheLayer layer = cache.DecompressTile(tile);
                var comp = _compFactory.Create(0);
                data = DtTileCacheBuilder.CompressTileCacheLayer(comp, layer);
                RcIO.Write(stream, data.Length);
                stream.Write(data);
            }
        }

        private void WriteCacheParams(BinaryWriter stream, DtTileCacheParams option)
        {
            RcIO.Write(stream, option.orig.X);
            RcIO.Write(stream, option.orig.Y);
            RcIO.Write(stream, option.orig.Z);

            RcIO.Write(stream, option.cs);
            RcIO.Write(stream, option.ch);
            RcIO.Write(stream, option.width);
            RcIO.Write(stream, option.height);
            RcIO.Write(stream, option.walkableHeight);
            RcIO.Write(stream, option.walkableRadius);
            RcIO.Write(stream, option.walkableClimb);
            RcIO.Write(stream, option.maxSimplificationError);
            RcIO.Write(stream, option.maxTiles);
            RcIO.Write(stream, option.maxObstacles);
        }
    }
}