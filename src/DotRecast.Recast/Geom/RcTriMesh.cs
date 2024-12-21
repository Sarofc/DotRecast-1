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

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using DotRecast.Core;

namespace DotRecast.Recast.Geom
{
    public class RcTriMesh
    {
        private readonly List<float> vertices;
        private readonly List<int> faces;
        public readonly RcChunkyTriMesh chunkyTriMesh;

        //private readonly ArrayPool<float>

        public static RcTriMesh Load(string filename)
        {
            using var stream = RcIO.ReadFileIfFound(filename);
            var context = RcObjImporter.LoadContext(stream);
            //Console.WriteLine($"{{context.capcatiy}} {context.vertexPositions.Count} {context.meshFaces.Count}");
            return new RcTriMesh(context.vertexPositions, context.meshFaces);
        }

        public RcTriMesh(RcTriMesh mesh, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            vertices = new List<float>(mesh.vertices.Count);
            faces = new List<int>(mesh.faces);

            CollectionsMarshal.SetCount(vertices, mesh.vertices.Count);
            for (int i = 0; i < faces.Count; i++)
            {
                var v = RcVec.Create(mesh.vertices, faces[i] * 3);

                var nv = Vector3.Transform(scale * (v + position), rotation);

                vertices[faces[i] * 3 + 0] = nv.X;
                vertices[faces[i] * 3 + 1] = nv.Y;
                vertices[faces[i] * 3 + 2] = nv.Z;
            }

            chunkyTriMesh = new RcChunkyTriMesh();
            RcChunkyTriMeshs.CreateChunkyTriMesh(GetVerts(), GetTris(), faces.Count / 3, 32, chunkyTriMesh);
        }

        public RcTriMesh(List<float> vertices, List<int> faces)
        {
            this.vertices = vertices;
            this.faces = faces;
            chunkyTriMesh = new RcChunkyTriMesh();
            RcChunkyTriMeshs.CreateChunkyTriMesh(GetVerts(), GetTris(), faces.Count / 3, 32, chunkyTriMesh);
        }

        public Span<int> GetTris()
        {
            return CollectionsMarshal.AsSpan(faces);
        }

        public Span<float> GetVerts()
        {
            return CollectionsMarshal.AsSpan(vertices);
        }

        public List<RcChunkyTriMeshNode> GetChunksOverlappingRect(Vector2 bmin, Vector2 bmax)
        {
            return RcChunkyTriMeshs.GetChunksOverlappingRect(chunkyTriMesh, bmin, bmax);
        }

        public override string ToString()
        {
            return $"vertices={vertices.Count} faces={faces.Count}";
        }
    }
}