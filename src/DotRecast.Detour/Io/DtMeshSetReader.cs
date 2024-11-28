/*
Recast4J Copyright (c) 2015 Piotr Piastucki piotr@jtilia.org

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

using System;
using System.IO;
using DotRecast.Core;

namespace DotRecast.Detour.Io
{
    using static DtDetour;

    public struct DtMeshSetReader
    {
        public DtNavMesh Read(BinaryReader @is, int maxVertPerPoly)
        {
            var bb = RcIO.ToByteBuffer(@is);
            return Read(ref bb, maxVertPerPoly, false);
        }

        public DtNavMesh Read(ref RcByteBuffer bb, int maxVertPerPoly)
        {
            return Read(ref bb, maxVertPerPoly, false);
        }

        public DtNavMesh Read32Bit(BinaryReader @is, int maxVertPerPoly)
        {
            var bb = RcIO.ToByteBuffer(@is);
            return Read(ref bb, maxVertPerPoly, true);
        }

        public DtNavMesh Read32Bit(ref RcByteBuffer bb, int maxVertPerPoly)
        {
            return Read(ref bb, maxVertPerPoly, true);
        }

        public DtNavMesh Read(BinaryReader @is)
        {
            var bb = RcIO.ToByteBuffer(@is);
            return Read(ref bb);
        }

        public DtNavMesh Read(ref RcByteBuffer bb)
        {
            return Read(ref bb, -1, false);
        }

        DtNavMesh Read(ref RcByteBuffer bb, int maxVertPerPoly, bool is32Bit)
        {
            NavMeshSetHeader header = ReadHeader(ref bb, maxVertPerPoly);
            if (header.maxVertsPerPoly <= 0)
            {
                throw new IOException("Invalid number of verts per poly " + header.maxVertsPerPoly);
            }

            bool cCompatibility = header.version == NavMeshSetHeader.NAVMESHSET_VERSION;
            DtNavMesh mesh = new DtNavMesh();
            mesh.Init(header.option, header.maxVertsPerPoly);
            ReadTiles(ref bb, is32Bit, ref header, cCompatibility, mesh);
            return mesh;
        }

        private NavMeshSetHeader ReadHeader(ref RcByteBuffer bb, int maxVertsPerPoly)
        {
            NavMeshSetHeader header = new NavMeshSetHeader();
            header.magic = bb.ReadInt32();
            if (header.magic != NavMeshSetHeader.NAVMESHSET_MAGIC)
            {
                header.magic = RcIO.SwapEndianness(header.magic);
                if (header.magic != NavMeshSetHeader.NAVMESHSET_MAGIC)
                {
                    throw new IOException("Invalid magic " + header.magic);
                }

                bb.Order(bb.Order() == RcByteOrder.BIG_ENDIAN ? RcByteOrder.LITTLE_ENDIAN : RcByteOrder.BIG_ENDIAN);
            }

            header.version = bb.ReadInt32();
            if (header.version != NavMeshSetHeader.NAVMESHSET_VERSION && header.version != NavMeshSetHeader.NAVMESHSET_VERSION_RECAST4J_1
                                                                      && header.version != NavMeshSetHeader.NAVMESHSET_VERSION_RECAST4J)
            {
                throw new IOException("Invalid version " + header.version);
            }

            header.numTiles = bb.ReadInt32();
            DtNavMeshParamsReader paramReader;
            header.option = paramReader.Read(ref bb);
            header.maxVertsPerPoly = maxVertsPerPoly;
            if (header.version == NavMeshSetHeader.NAVMESHSET_VERSION_RECAST4J)
            {
                header.maxVertsPerPoly = bb.ReadInt32();
            }

            return header;
        }

        private void ReadTiles(ref RcByteBuffer bb, bool is32Bit, ref NavMeshSetHeader header, bool cCompatibility, DtNavMesh mesh)
        {
            // Read tiles.
            for (int i = 0; i < header.numTiles; ++i)
            {
                NavMeshTileHeader tileHeader = new NavMeshTileHeader();
                if (is32Bit)
                {
                    tileHeader.tileRef = Convert32BitRef(bb.ReadInt32(), header.option);
                }
                else
                {
                    tileHeader.tileRef = bb.ReadInt64();
                }

                tileHeader.dataSize = bb.ReadInt32();
                if (tileHeader.tileRef == 0 || tileHeader.dataSize == 0)
                {
                    break;
                }

                if (cCompatibility && !is32Bit)
                {
                    bb.ReadInt32(); // C struct padding
                }

                DtMeshDataReader meshReader;
                DtMeshData data = meshReader.Read(ref bb, mesh.GetMaxVertsPerPoly(), is32Bit);
                mesh.AddTile(data, i, tileHeader.tileRef, out _);
            }
        }

        private long Convert32BitRef(int refs, DtNavMeshParams option)
        {
            int m_tileBits = DtUtils.Ilog2(DtUtils.NextPow2(option.maxTiles));
            int m_polyBits = DtUtils.Ilog2(DtUtils.NextPow2(option.maxPolys));
            // Only allow 31 salt bits, since the salt mask is calculated using 32bit uint and it will overflow.
            int m_saltBits = Math.Min(31, 32 - m_tileBits - m_polyBits);
            int saltMask = (1 << m_saltBits) - 1;
            int tileMask = (1 << m_tileBits) - 1;
            int polyMask = (1 << m_polyBits) - 1;
            int salt = ((refs >> (m_polyBits + m_tileBits)) & saltMask);
            int it = ((refs >> m_polyBits) & tileMask);
            int ip = refs & polyMask;
            return EncodePolyId(salt, it, ip);
        }
    }
}