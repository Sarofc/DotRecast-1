/*
Recast4J Copyright (c) 2015-2018 Piotr Piastucki piotr@jtilia.org

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

namespace DotRecast.Detour.Io
{
    public struct DtMeshSetWriter
    {
        public void Write(BinaryWriter stream, DtNavMesh mesh)
        {
            WriteHeader(stream, mesh);
            WriteTiles(stream, mesh);
        }

        private void WriteHeader(BinaryWriter stream, DtNavMesh mesh)
        {
            RcIO.Write(stream, NavMeshSetHeader.NAVMESHSET_MAGIC);
            RcIO.Write(stream, NavMeshSetHeader.NAVMESHSET_VERSION);
            int numTiles = 0;
            for (int i = 0; i < mesh.GetMaxTiles(); ++i)
            {
                DtMeshTile tile = mesh.GetTile(i);
                if (tile == null || tile.data == null || tile.data.header == null)
                {
                    continue;
                }

                numTiles++;
            }

            RcIO.Write(stream, numTiles);
            DtNavMeshParamWriter paramWriter;
            paramWriter.Write(stream, mesh.GetParams());
            //if (!cCompatibility)
            //{
            //    RcIO.Write(stream, mesh.GetMaxVertsPerPoly());
            //}
        }

        private void WriteTiles(BinaryWriter stream, DtNavMesh mesh)
        {
            for (int i = 0; i < mesh.GetMaxTiles(); ++i)
            {
                DtMeshTile tile = mesh.GetTile(i);
                if (tile == null || tile.data == null || tile.data.header == null)
                {
                    continue;
                }

                NavMeshTileHeader tileHeader = new NavMeshTileHeader();
                tileHeader.tileRef = mesh.GetTileRef(tile);
                using MemoryStream msw = new MemoryStream();
                using BinaryWriter bw = new BinaryWriter(msw);
                DtMeshDataWriter writer;
                writer.Write(bw, tile.data);
                bw.Flush();
                bw.Close();

                byte[] ba = msw.ToArray();
                tileHeader.dataSize = ba.Length;
                RcIO.Write(stream, tileHeader.tileRef);
                RcIO.Write(stream, tileHeader.dataSize);
                //if (cCompatibility)
                //{
                //    RcIO.Write(stream, 0); // C struct padding
                //}

                stream.Write(ba);
            }
        }
    }
}