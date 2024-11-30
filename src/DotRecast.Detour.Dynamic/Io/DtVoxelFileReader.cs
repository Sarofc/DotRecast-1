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
using System.Numerics;
using DotRecast.Detour.Io;

namespace DotRecast.Detour.Dynamic.Io
{
    public class DtVoxelFileReader
    {
        private readonly IRcCompressor _compressor;

        public DtVoxelFileReader(IRcCompressor compressor)
        {
            _compressor = compressor;
        }

        public DtVoxelFile Read(BinaryReader stream)
        {
            RcByteBuffer buf = RcIO.ToByteBuffer(stream);
            DtVoxelFile file = new DtVoxelFile();
            int magic = buf.ReadInt32();
            if (magic != DtVoxelFile.MAGIC)
            {
                //magic = RcIO.SwapEndianness(magic);
                if (magic != DtVoxelFile.MAGIC)
                {
                    throw new IOException("Invalid magic");
                }
            }

            file.version = buf.ReadInt32();
            bool isExportedFromAstar = (file.version & DtVoxelFile.VERSION_EXPORTER_MASK) == 0;
            bool compression = (file.version & DtVoxelFile.VERSION_COMPRESSION_MASK) == DtVoxelFile.VERSION_COMPRESSION_LZ4;
            file.walkableRadius = buf.ReadSingle();
            file.walkableHeight = buf.ReadSingle();
            file.walkableClimb = buf.ReadSingle();
            file.walkableSlopeAngle = buf.ReadSingle();
            file.cellSize = buf.ReadSingle();
            file.maxSimplificationError = buf.ReadSingle();
            file.maxEdgeLen = buf.ReadSingle();
            file.minRegionArea = (int)buf.ReadSingle();
            if (!isExportedFromAstar)
            {
                file.regionMergeArea = buf.ReadSingle();
                file.vertsPerPoly = buf.ReadInt32();
                file.buildMeshDetail = buf.ReadByte() != 0;
                file.detailSampleDistance = buf.ReadSingle();
                file.detailSampleMaxError = buf.ReadSingle();
            }
            else
            {
                file.regionMergeArea = 6 * file.minRegionArea;
                file.vertsPerPoly = 6;
                file.buildMeshDetail = true;
                file.detailSampleDistance = file.maxEdgeLen * 0.5f;
                file.detailSampleMaxError = file.maxSimplificationError * 0.8f;
            }

            file.useTiles = buf.ReadByte() != 0;
            file.tileSizeX = buf.ReadInt32();
            file.tileSizeZ = buf.ReadInt32();
            file.rotation.X = buf.ReadSingle();
            file.rotation.Y = buf.ReadSingle();
            file.rotation.Z = buf.ReadSingle();
            file.bounds[0] = buf.ReadSingle();
            file.bounds[1] = buf.ReadSingle();
            file.bounds[2] = buf.ReadSingle();
            file.bounds[3] = buf.ReadSingle();
            file.bounds[4] = buf.ReadSingle();
            file.bounds[5] = buf.ReadSingle();
            if (isExportedFromAstar)
            {
                // bounds are saved as center + size
                file.bounds[0] -= 0.5f * file.bounds[3];
                file.bounds[1] -= 0.5f * file.bounds[4];
                file.bounds[2] -= 0.5f * file.bounds[5];
                file.bounds[3] += file.bounds[0];
                file.bounds[4] += file.bounds[1];
                file.bounds[5] += file.bounds[2];
            }

            int tileCount = buf.ReadInt32();
            for (int tile = 0; tile < tileCount; tile++)
            {
                int tileX = buf.ReadInt32();
                int tileZ = buf.ReadInt32();
                int width = buf.ReadInt32();
                int depth = buf.ReadInt32();
                int borderSize = buf.ReadInt32();
                Vector3 boundsMin = new Vector3();
                boundsMin.X = buf.ReadSingle();
                boundsMin.Y = buf.ReadSingle();
                boundsMin.Z = buf.ReadSingle();
                Vector3 boundsMax = new Vector3();
                boundsMax.X = buf.ReadSingle();
                boundsMax.Y = buf.ReadSingle();
                boundsMax.Z = buf.ReadSingle();
                if (isExportedFromAstar)
                {
                    // bounds are local
                    boundsMin.X += file.bounds[0];
                    boundsMin.Y += file.bounds[1];
                    boundsMin.Z += file.bounds[2];
                    boundsMax.X += file.bounds[0];
                    boundsMax.Y += file.bounds[1];
                    boundsMax.Z += file.bounds[2];
                }

                float cellSize = buf.ReadSingle();
                float cellHeight = buf.ReadSingle();
                int voxelSize = buf.ReadInt32();
                int position = buf.Position();
                byte[] bytes = buf.ReadBytes(voxelSize).ToArray();
                if (compression)
                {
                    bytes = _compressor.Decompress(bytes);
                }

                RcByteBuffer data = new RcByteBuffer(bytes);
                file.AddTile(new DtVoxelTile(tileX, tileZ, width, depth, boundsMin, boundsMax, cellSize, cellHeight, borderSize, ref data));
                buf.Position(position + voxelSize);
            }

            return file;
        }
    }
}