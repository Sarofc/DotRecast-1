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

using System.IO;
using DotRecast.Core;

namespace DotRecast.Detour.Io
{
    public struct DtMeshSetReader
    {
        public DtNavMesh Read(BinaryReader @is, int maxVertPerPoly)
        {
            var bb = RcIO.ToByteBuffer(@is);
            return Read(ref bb, maxVertPerPoly);
        }

        public DtNavMesh Read(BinaryReader @is)
        {
            var bb = RcIO.ToByteBuffer(@is);
            return Read(ref bb);
        }

        public DtNavMesh Read(ref RcByteBuffer bb)
        {
            return Read(ref bb, -1);
        }

        DtNavMesh Read(ref RcByteBuffer bb, int maxVertPerPoly)
        {
            NavMeshSetHeader header = ReadHeader(ref bb, maxVertPerPoly);
            if (header.maxVertsPerPoly <= 0)
            {
                throw new IOException("Invalid number of verts per poly " + header.maxVertsPerPoly);
            }

            DtNavMesh mesh = new();
            mesh.Init(header.option, header.maxVertsPerPoly);
            ReadTiles(ref bb, ref header, mesh);
            return mesh;
        }

        private NavMeshSetHeader ReadHeader(ref RcByteBuffer bb, int maxVertsPerPoly)
        {
            NavMeshSetHeader header = new();
            header.magic = bb.ReadInt32();
            if (header.magic != NavMeshSetHeader.NAVMESHSET_MAGIC)
            {
                throw new IOException("Invalid magic " + header.magic);
            }

            header.version = bb.ReadInt32();
            if (header.version != NavMeshSetHeader.NAVMESHSET_VERSION)
            {
                throw new IOException("Invalid version " + header.version);
            }

            header.numTiles = bb.ReadInt32();
            DtNavMeshParamsReader paramReader;
            header.option = paramReader.Read(ref bb);
            header.maxVertsPerPoly = maxVertsPerPoly;

            return header;
        }

        private void ReadTiles(ref RcByteBuffer bb, ref NavMeshSetHeader header, DtNavMesh mesh)
        {
            // Read tiles.
            for (int i = 0; i < header.numTiles; ++i)
            {
                NavMeshTileHeader tileHeader = new();

                tileHeader.tileRef = bb.ReadInt64();

                tileHeader.dataSize = bb.ReadInt32();
                if (tileHeader.tileRef == 0 || tileHeader.dataSize == 0)
                {
                    break;
                }

                DtMeshDataReader meshReader;
                DtMeshData data = meshReader.Read(ref bb, mesh.GetMaxVertsPerPoly());
                mesh.AddTile(data, i, tileHeader.tileRef, out _);
            }
        }
    }
}