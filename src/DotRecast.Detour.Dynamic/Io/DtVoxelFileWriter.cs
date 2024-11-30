/*
recast4j copyright (c) 2021 Piotr Piastucki piotr@jtilia.org
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

namespace DotRecast.Detour.Dynamic.Io
{
    public struct DtVoxelFileWriter
    {
        private readonly IRcCompressor _compressor;

        public DtVoxelFileWriter(IRcCompressor compressor)
        {
            _compressor = compressor;
        }

        public void Write(BinaryWriter stream, DtVoxelFile f, bool compression)
        {
            RcIO.Write(stream, DtVoxelFile.MAGIC);
            RcIO.Write(stream, (compression ? DtVoxelFile.VERSION_COMPRESSION_LZ4 : 0));
            RcIO.Write(stream, f.walkableRadius);
            RcIO.Write(stream, f.walkableHeight);
            RcIO.Write(stream, f.walkableClimb);
            RcIO.Write(stream, f.walkableSlopeAngle);
            RcIO.Write(stream, f.cellSize);
            RcIO.Write(stream, f.maxSimplificationError);
            RcIO.Write(stream, f.maxEdgeLen);
            RcIO.Write(stream, f.minRegionArea);
            RcIO.Write(stream, f.regionMergeArea);
            RcIO.Write(stream, f.vertsPerPoly);
            RcIO.Write(stream, f.buildMeshDetail);
            RcIO.Write(stream, f.detailSampleDistance);
            RcIO.Write(stream, f.detailSampleMaxError);
            RcIO.Write(stream, f.useTiles);
            RcIO.Write(stream, f.tileSizeX);
            RcIO.Write(stream, f.tileSizeZ);
            RcIO.Write(stream, f.rotation.X);
            RcIO.Write(stream, f.rotation.Y);
            RcIO.Write(stream, f.rotation.Z);
            RcIO.Write(stream, f.bounds[0]);
            RcIO.Write(stream, f.bounds[1]);
            RcIO.Write(stream, f.bounds[2]);
            RcIO.Write(stream, f.bounds[3]);
            RcIO.Write(stream, f.bounds[4]);
            RcIO.Write(stream, f.bounds[5]);
            RcIO.Write(stream, f.tiles.Count);
            foreach (DtVoxelTile t in f.tiles)
            {
                WriteTile(stream, t, compression);
            }
        }

        public void WriteTile(BinaryWriter stream, DtVoxelTile tile, bool compression)
        {
            RcIO.Write(stream, tile.tileX);
            RcIO.Write(stream, tile.tileZ);
            RcIO.Write(stream, tile.width);
            RcIO.Write(stream, tile.depth);
            RcIO.Write(stream, tile.borderSize);
            RcIO.Write(stream, tile.boundsMin.X);
            RcIO.Write(stream, tile.boundsMin.Y);
            RcIO.Write(stream, tile.boundsMin.Z);
            RcIO.Write(stream, tile.boundsMax.X);
            RcIO.Write(stream, tile.boundsMax.Y);
            RcIO.Write(stream, tile.boundsMax.Z);
            RcIO.Write(stream, tile.cellSize);
            RcIO.Write(stream, tile.cellHeight);
            byte[] bytes = tile.spanData;
            if (compression)
            {
                bytes = _compressor.Compress(bytes);
            }

            RcIO.Write(stream, bytes.Length);
            stream.Write(bytes);
        }
    }
}