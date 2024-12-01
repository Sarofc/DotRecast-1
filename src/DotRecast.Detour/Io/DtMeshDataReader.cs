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
    using static DtDetour;

    public struct DtMeshDataReader
    {
        public DtMeshData Read(BinaryReader stream, int maxVertPerPoly)
        {
            RcByteBuffer buf = RcIO.ToByteBuffer(stream);
            return Read(ref buf, maxVertPerPoly);
        }

        public DtMeshData Read(ref RcByteBuffer buf, int maxVertPerPoly)
        {
            DtMeshData data = new DtMeshData();
            DtMeshHeader header = new DtMeshHeader();
            data.header = header;
            header.magic = buf.ReadInt32();
            if (header.magic != DT_NAVMESH_MAGIC)
            {
                throw new IOException("Invalid magic");
            }

            header.version = buf.ReadInt32();
            if (header.version != DT_NAVMESH_VERSION)
            {
                throw new IOException("Invalid version " + header.version);
            }

            header.x = buf.ReadInt32();
            header.y = buf.ReadInt32();
            header.layer = buf.ReadInt32();
            header.userId = buf.ReadInt32();
            header.polyCount = buf.ReadInt32();
            header.vertCount = buf.ReadInt32();
            header.maxLinkCount = buf.ReadInt32();
            header.detailMeshCount = buf.ReadInt32();
            header.detailVertCount = buf.ReadInt32();
            header.detailTriCount = buf.ReadInt32();
            header.bvNodeCount = buf.ReadInt32();
            header.offMeshConCount = buf.ReadInt32();
            header.offMeshBase = buf.ReadInt32();
            header.walkableHeight = buf.ReadSingle();
            header.walkableRadius = buf.ReadSingle();
            header.walkableClimb = buf.ReadSingle();

            header.bmin.X = buf.ReadSingle();
            header.bmin.Y = buf.ReadSingle();
            header.bmin.Z = buf.ReadSingle();

            header.bmax.X = buf.ReadSingle();
            header.bmax.Y = buf.ReadSingle();
            header.bmax.Z = buf.ReadSingle();

            header.bvQuantFactor = buf.ReadSingle();
            data.verts = ReadVerts(ref buf, header.vertCount);
            data.polys = ReadPolys(ref buf, header, maxVertPerPoly);

            data.detailMeshes = ReadPolyDetails(ref buf, header);
            data.detailVerts = ReadVerts(ref buf, header.detailVertCount);
            data.detailTris = ReadDTris(ref buf, header);
            data.bvTree = ReadBVTree(ref buf, header);
            data.offMeshCons = ReadOffMeshCons(ref buf, header);
            return data;
        }

        private float[] ReadVerts(ref RcByteBuffer buf, int count)
        {
            float[] verts = new float[count * 3];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = buf.ReadSingle();
            }

            return verts;
        }

        private DtPoly[] ReadPolys(ref RcByteBuffer buf, DtMeshHeader header, int maxVertPerPoly)
        {
            DtPoly[] polys = new DtPoly[header.polyCount];
            for (int i = 0; i < polys.Length; i++)
            {
                polys[i] = new DtPoly(i, maxVertPerPoly);

                for (int j = 0; j < polys[i].verts.Length; j++)
                {
                    polys[i].verts[j] = buf.ReadInt16() & 0xFFFF;
                }

                for (int j = 0; j < polys[i].neis.Length; j++)
                {
                    polys[i].neis[j] = buf.ReadInt16() & 0xFFFF;
                }

                polys[i].flags = buf.ReadInt16() & 0xFFFF;
                polys[i].vertCount = buf.ReadByte() & 0xFF;
                polys[i].areaAndtype = buf.ReadByte() & 0xFF;
            }

            return polys;
        }

        private DtPolyDetail[] ReadPolyDetails(ref RcByteBuffer buf, DtMeshHeader header)
        {
            DtPolyDetail[] polys = new DtPolyDetail[header.detailMeshCount];
            for (int i = 0; i < polys.Length; i++)
            {
                int vertBase = buf.ReadInt32();
                int triBase = buf.ReadInt32();
                byte vertCount = (byte)(buf.ReadByte() & 0xFF);
                byte triCount = (byte)(buf.ReadByte() & 0xFF);
                polys[i] = new DtPolyDetail(vertBase, triBase, vertCount, triCount);
            }

            return polys;
        }

        private int[] ReadDTris(ref RcByteBuffer buf, DtMeshHeader header)
        {
            int[] tris = new int[4 * header.detailTriCount];
            for (int i = 0; i < tris.Length; i++)
            {
                tris[i] = buf.ReadByte() & 0xFF;
            }

            return tris;
        }

        private unsafe DtBVNode[] ReadBVTree(ref RcByteBuffer buf, DtMeshHeader header)
        {
            DtBVNode[] nodes = new DtBVNode[header.bvNodeCount];
            for (int i = 0; i < nodes.Length; i++)
            {
                ref var n = ref nodes[i];

                for (int j = 0; j < 3; j++)
                {
                    n.bmin[j] = buf.ReadInt16() & 0xFFFF;
                }

                for (int j = 0; j < 3; j++)
                {
                    n.bmax[j] = buf.ReadInt16() & 0xFFFF;
                }

                n.i = buf.ReadInt32();
            }

            return nodes;
        }

        private unsafe DtOffMeshConnection[] ReadOffMeshCons(ref RcByteBuffer buf, DtMeshHeader header)
        {
            DtOffMeshConnection[] cons = new DtOffMeshConnection[header.offMeshConCount];
            for (int i = 0; i < cons.Length; i++)
            {
                ref DtOffMeshConnection con = ref cons[i];
                for (int j = 0; j < 2; j++)
                {
                    con.pos[j * 3 + 0] = buf.ReadSingle();
                    con.pos[j * 3 + 1] = buf.ReadSingle();
                    con.pos[j * 3 + 2] = buf.ReadSingle();
                }

                cons[i].rad = buf.ReadSingle();
                cons[i].poly = buf.ReadInt16() & 0xFFFF;
                cons[i].flags = buf.ReadByte() & 0xFF;
                cons[i].side = buf.ReadByte() & 0xFF;
                cons[i].userId = buf.ReadInt32();
            }

            return cons;
        }
    }
}