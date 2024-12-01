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

    public struct DtMeshDataWriter
    {
        public void Write(BinaryWriter stream, DtMeshData data)
        {
            DtMeshHeader header = data.header;
            RcIO.Write(stream, header.magic);
            RcIO.Write(stream, DT_NAVMESH_VERSION);
            RcIO.Write(stream, header.x);
            RcIO.Write(stream, header.y);
            RcIO.Write(stream, header.layer);
            RcIO.Write(stream, header.userId);
            RcIO.Write(stream, header.polyCount);
            RcIO.Write(stream, header.vertCount);
            RcIO.Write(stream, header.maxLinkCount);
            RcIO.Write(stream, header.detailMeshCount);
            RcIO.Write(stream, header.detailVertCount);
            RcIO.Write(stream, header.detailTriCount);
            RcIO.Write(stream, header.bvNodeCount);
            RcIO.Write(stream, header.offMeshConCount);
            RcIO.Write(stream, header.offMeshBase);
            RcIO.Write(stream, header.walkableHeight);
            RcIO.Write(stream, header.walkableRadius);
            RcIO.Write(stream, header.walkableClimb);
            RcIO.Write(stream, header.bmin.X);
            RcIO.Write(stream, header.bmin.Y);
            RcIO.Write(stream, header.bmin.Z);
            RcIO.Write(stream, header.bmax.X);
            RcIO.Write(stream, header.bmax.Y);
            RcIO.Write(stream, header.bmax.Z);
            RcIO.Write(stream, header.bvQuantFactor);
            WriteVerts(stream, data.verts, header.vertCount);
            WritePolys(stream, data);

            WritePolyDetails(stream, data);
            WriteVerts(stream, data.detailVerts, header.detailVertCount);
            WriteDTris(stream, data);
            WriteBVTree(stream, data);
            WriteOffMeshCons(stream, data);
        }

        private void WriteVerts(BinaryWriter stream, float[] verts, int count)
        {
            for (int i = 0; i < count * 3; i++)
            {
                RcIO.Write(stream, verts[i]);
            }
        }

        private void WritePolys(BinaryWriter stream, DtMeshData data)
        {
            for (int i = 0; i < data.header.polyCount; i++)
            {
                for (int j = 0; j < data.polys[i].verts.Length; j++)
                {
                    RcIO.Write(stream, (short)data.polys[i].verts[j]);
                }

                for (int j = 0; j < data.polys[i].neis.Length; j++)
                {
                    RcIO.Write(stream, (short)data.polys[i].neis[j]);
                }

                RcIO.Write(stream, (short)data.polys[i].flags);
                RcIO.Write(stream, (byte)data.polys[i].vertCount);
                RcIO.Write(stream, (byte)data.polys[i].areaAndtype);
            }
        }

        private void WritePolyDetails(BinaryWriter stream, DtMeshData data)
        {
            for (int i = 0; i < data.header.detailMeshCount; i++)
            {
                RcIO.Write(stream, data.detailMeshes[i].vertBase);
                RcIO.Write(stream, data.detailMeshes[i].triBase);
                RcIO.Write(stream, (byte)data.detailMeshes[i].vertCount);
                RcIO.Write(stream, (byte)data.detailMeshes[i].triCount);
            }
        }

        private void WriteDTris(BinaryWriter stream, DtMeshData data)
        {
            for (int i = 0; i < data.header.detailTriCount * 4; i++)
            {
                RcIO.Write(stream, (byte)data.detailTris[i]);
            }
        }

        private unsafe void WriteBVTree(BinaryWriter stream, DtMeshData data)
        {
            for (int i = 0; i < data.header.bvNodeCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    RcIO.Write(stream, (short)data.bvTree[i].bmin[j]);
                }

                for (int j = 0; j < 3; j++)
                {
                    RcIO.Write(stream, (short)data.bvTree[i].bmax[j]);
                }

                RcIO.Write(stream, data.bvTree[i].i);
            }
        }

        private unsafe void WriteOffMeshCons(BinaryWriter stream, DtMeshData data)
        {
            for (int i = 0; i < data.header.offMeshConCount; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    RcIO.Write(stream, data.offMeshCons[i].pos[j * 3 + 0]);
                    RcIO.Write(stream, data.offMeshCons[i].pos[j * 3 + 1]);
                    RcIO.Write(stream, data.offMeshCons[i].pos[j * 3 + 2]);
                }

                RcIO.Write(stream, data.offMeshCons[i].rad);
                RcIO.Write(stream, (short)data.offMeshCons[i].poly);
                RcIO.Write(stream, (byte)data.offMeshCons[i].flags);
                RcIO.Write(stream, (byte)data.offMeshCons[i].side);
                RcIO.Write(stream, data.offMeshCons[i].userId);
            }
        }
    }
}