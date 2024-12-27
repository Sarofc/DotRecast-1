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
        public DtMeshData Read(BinaryReader br, int maxVertPerPoly)
        {
            DtMeshData data = new();
            DtMeshHeader header = new();
            data.header = header;
            header.magic = br.ReadInt32();
            if (header.magic != DT_NAVMESH_MAGIC)
            {
                throw new IOException("Invalid magic");
            }

            header.version = br.ReadInt32();
            if (header.version != DT_NAVMESH_VERSION)
            {
                throw new IOException("Invalid version " + header.version);
            }

            header.x = br.ReadInt32();
            header.y = br.ReadInt32();
            header.layer = br.ReadInt32();
            header.userId = br.ReadInt32();
            header.polyCount = br.ReadInt32();
            header.vertCount = br.ReadInt32();
            header.maxLinkCount = br.ReadInt32();
            header.detailMeshCount = br.ReadInt32();
            header.detailVertCount = br.ReadInt32();
            header.detailTriCount = br.ReadInt32();
            header.bvNodeCount = br.ReadInt32();
            header.offMeshConCount = br.ReadInt32();
            header.offMeshBase = br.ReadInt32();
            header.walkableHeight = br.ReadSingle();
            header.walkableRadius = br.ReadSingle();
            header.walkableClimb = br.ReadSingle();

            header.bmin.X = br.ReadSingle();
            header.bmin.Y = br.ReadSingle();
            header.bmin.Z = br.ReadSingle();

            header.bmax.X = br.ReadSingle();
            header.bmax.Y = br.ReadSingle();
            header.bmax.Z = br.ReadSingle();

            header.bvQuantFactor = br.ReadSingle();
            data.verts = ReadVerts(br, header.vertCount);
            data.polys = ReadPolys(br, header, maxVertPerPoly);

            data.detailMeshes = ReadPolyDetails(br, header);
            data.detailVerts = ReadVerts(br, header.detailVertCount);
            data.detailTris = ReadDTris(br, header);
            data.bvTree = ReadBVTree(br, header);
            data.offMeshCons = ReadOffMeshCons(br, header);
            return data;
        }

        private float[] ReadVerts(BinaryReader br, int count)
        {
            float[] verts = new float[count * 3];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = br.ReadSingle();
            }

            return verts;
        }

        private DtPoly[] ReadPolys(BinaryReader br, DtMeshHeader header, int maxVertPerPoly)
        {
            DtPoly[] polys = new DtPoly[header.polyCount];
            for (int i = 0; i < polys.Length; i++)
            {
                polys[i] = new DtPoly(i, maxVertPerPoly);

                for (int j = 0; j < polys[i].verts.Length; j++)
                {
                    polys[i].verts[j] = br.ReadUInt16();
                }

                for (int j = 0; j < polys[i].neis.Length; j++)
                {
                    polys[i].neis[j] = br.ReadUInt16();
                }

                polys[i].flags = br.ReadUInt16();
                polys[i].vertCount = br.ReadByte();
                polys[i].areaAndtype = br.ReadByte();
            }

            return polys;
        }

        private DtPolyDetail[] ReadPolyDetails(BinaryReader buf, DtMeshHeader header)
        {
            DtPolyDetail[] polys = new DtPolyDetail[header.detailMeshCount];
            for (int i = 0; i < polys.Length; i++)
            {
                int vertBase = buf.ReadInt32();
                int triBase = buf.ReadInt32();
                byte vertCount = buf.ReadByte();
                byte triCount = buf.ReadByte();
                polys[i] = new DtPolyDetail(vertBase, triBase, vertCount, triCount);
            }

            return polys;
        }

        private byte[] ReadDTris(BinaryReader buf, DtMeshHeader header)
        {
            byte[] tris = new byte[4 * header.detailTriCount];
            for (int i = 0; i < tris.Length; i++)
            {
                tris[i] = buf.ReadByte();
            }

            return tris;
        }

        private unsafe DtBVNode[] ReadBVTree(BinaryReader buf, DtMeshHeader header)
        {
            DtBVNode[] nodes = new DtBVNode[header.bvNodeCount];
            for (int i = 0; i < nodes.Length; i++)
            {
                ref var n = ref nodes[i];

                for (int j = 0; j < 3; j++)
                {
                    n.bmin[j] = buf.ReadUInt16();
                }

                for (int j = 0; j < 3; j++)
                {
                    n.bmax[j] = buf.ReadUInt16();
                }

                n.i = buf.ReadInt32();
            }

            return nodes;
        }

        private unsafe DtOffMeshConnection[] ReadOffMeshCons(BinaryReader buf, DtMeshHeader header)
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
                cons[i].poly = buf.ReadUInt16();
                cons[i].flags = buf.ReadByte();
                cons[i].side = buf.ReadByte();
                cons[i].userId = buf.ReadInt32();
            }

            return cons;
        }
    }
}