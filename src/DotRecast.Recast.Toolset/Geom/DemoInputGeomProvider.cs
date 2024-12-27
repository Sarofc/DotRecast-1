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
using System.Collections.Generic;
using System.Numerics;
using DotRecast.Core;
using DotRecast.Recast.Geom;

namespace DotRecast.Recast.Toolset.Geom
{
    public class DemoInputGeomProvider : IInputGeomProvider
    {
        const int MAX_OFFMESH_CONNECTIONS = 256;
        public int OffMeshConCount => m_offMeshConCount;
        public float[] OffMeshConVerts { get; } = new float[MAX_OFFMESH_CONNECTIONS * 3 * 2];
        public float[] OffMeshConRads { get; } = new float[MAX_OFFMESH_CONNECTIONS];
        public bool[] OffMeshConDirs { get; } = new bool[MAX_OFFMESH_CONNECTIONS];
        public byte[] OffMeshConAreas { get; } = new byte[MAX_OFFMESH_CONNECTIONS];
        public ushort[] OffMeshConFlags { get; } = new ushort[MAX_OFFMESH_CONNECTIONS];
        public int[] OffMeshConId { get; } = new int[MAX_OFFMESH_CONNECTIONS];
        int m_offMeshConCount;

        private Vector3 _bmin;
        private Vector3 _bmax;

        private readonly List<RcConvexVolume> _convexVolumes = new();

        private readonly List<RcTriMesh> _meshes = new();
        private readonly Dictionary<int, float[]> _normals = new();

        public DemoInputGeomProvider()
        { }

        public DemoInputGeomProvider(params ReadOnlySpan<RcTriMesh> meshes)
        {
            foreach (var mesh in meshes)
            {
                AddTriMesh(mesh);
            }
        }

        public void AddTriMesh(RcTriMesh mesh)
        {
            var vertices = mesh.GetVerts();

            for (int i = 0; i < vertices.Length / 3; i++)
            {
                _bmin = Vector3.Min(_bmin, RcVec.Create(vertices, i * 3));
                _bmax = Vector3.Max(_bmax, RcVec.Create(vertices, i * 3));
            }

            _meshes.Add(mesh);
        }

        public RcTriMesh GetMesh(int index) => _meshes[index];

        public List<RcTriMesh> Meshes() => _meshes;

        public float[] GetNormals(int index)
        {
            // 懒加载，normal只有绘制时才有用
            if (!_normals.TryGetValue(index, out var normals))
            {
                var mesh = GetMesh(index);

                var vertices = mesh.GetVerts();
                var faces = mesh.GetTris();

                normals = new float[faces.Length];
                _normals.Add(index, normals);
                CalculateNormals(normals, vertices, faces);
            }
            return normals;
        }

        public Vector3 GetMeshBoundsMin() => _bmin;
        public Vector3 GetMeshBoundsMax() => _bmax;

        public List<RcConvexVolume> ConvexVolumes() => _convexVolumes;

        public void AddOffMeshConnection(Vector3 spos, Vector3 epos, float radius, bool bidir, int area, int flags)
        {
            if (m_offMeshConCount >= MAX_OFFMESH_CONNECTIONS)
                return;
            Span<float> v = OffMeshConVerts.AsSpan(m_offMeshConCount * 3 * 2);
            OffMeshConRads[m_offMeshConCount] = radius;
            OffMeshConDirs[m_offMeshConCount] = bidir;
            OffMeshConAreas[m_offMeshConCount] = (byte)area;
            OffMeshConFlags[m_offMeshConCount] = (ushort)flags;
            OffMeshConId[m_offMeshConCount] = 1000 + m_offMeshConCount;
            spos.CopyTo(v);
            epos.CopyTo(v.Slice(3));
            m_offMeshConCount++;
        }

        public void RemoveOffMeshConnection(int i)
        {
            m_offMeshConCount--;
            var src = OffMeshConVerts.AsSpan(m_offMeshConCount * 3 * 2);
            var dst = OffMeshConVerts.AsSpan(i * 3 * 2);
            RcVec.Copy(dst, src);
            RcVec.Copy(dst.Slice(3), src.Slice(3));
            OffMeshConRads[i] = OffMeshConRads[m_offMeshConCount];
            OffMeshConDirs[i] = OffMeshConDirs[m_offMeshConCount];
            OffMeshConAreas[i] = OffMeshConAreas[m_offMeshConCount];
            OffMeshConFlags[i] = OffMeshConFlags[m_offMeshConCount];
        }

        public void AddConvexVolume(float[] verts, float minh, float maxh, RcAreaModification areaMod)
        {
            RcConvexVolume volume = new();
            volume.verts = verts;
            volume.hmin = minh;
            volume.hmax = maxh;
            volume.areaMod = areaMod;
            AddConvexVolume(volume);
        }

        public void AddConvexVolume(RcConvexVolume volume) => _convexVolumes.Add(volume);

        public void ClearConvexVolumes() => _convexVolumes.Clear();

        public bool RaycastMesh(Vector3 src, Vector3 dst, out float tmin)
        {
            foreach (var mesh in _meshes)
            {
                bool hit = RaycastMesh(mesh, _bmin, _bmax, src, dst, out tmin);
                if (hit)
                    return true;
            }

            tmin = 1.0f;
            return false;
        }

        public static bool RaycastMesh(RcTriMesh mesh, Vector3 bmin, Vector3 bmax, Vector3 src, Vector3 dst, out float tmin)
        {
            tmin = 1.0f;

            // Prune hit ray.
            if (!RcIntersections.IsectSegAABB(src, dst, bmin, bmax, out var btmin, out var btmax))
            {
                return false;
            }

            var p = new Vector2();
            var q = new Vector2();
            p.X = src.X + (dst.X - src.X) * btmin;
            p.Y = src.Z + (dst.Z - src.Z) * btmin;
            q.X = src.X + (dst.X - src.X) * btmax;
            q.Y = src.Z + (dst.Z - src.Z) * btmax;

            List<RcChunkyTriMeshNode> chunks = RcChunkyTriMeshs.GetChunksOverlappingSegment(mesh.chunkyTriMesh, p, q);
            if (0 == chunks.Count)
            {
                return false;
            }

            var vertices = mesh.GetVerts();

            tmin = 1.0f;
            bool hit = false;
            foreach (RcChunkyTriMeshNode chunk in chunks)
            {
                int[] tris = chunk.tris;
                for (int j = 0; j < chunk.tris.Length; j += 3)
                {
                    Vector3 v1 = new(
                        vertices[tris[j] * 3],
                        vertices[tris[j] * 3 + 1],
                        vertices[tris[j] * 3 + 2]
                    );
                    Vector3 v2 = new(
                        vertices[tris[j + 1] * 3],
                        vertices[tris[j + 1] * 3 + 1],
                        vertices[tris[j + 1] * 3 + 2]
                    );
                    Vector3 v3 = new(
                        vertices[tris[j + 2] * 3],
                        vertices[tris[j + 2] * 3 + 1],
                        vertices[tris[j + 2] * 3 + 2]
                    );
                    if (RcIntersections.IntersectSegmentTriangle(src, dst, v1, v2, v3, out var t))
                    {
                        if (t < tmin)
                        {
                            tmin = t;
                        }

                        hit = true;
                    }
                }
            }

            return hit;
        }


        static void CalculateNormals(Span<float> normals, ReadOnlySpan<float> vertices, ReadOnlySpan<int> faces)
        {
            for (int i = 0; i < faces.Length; i += 3)
            {
                Vector3 v0 = RcVec.Create(vertices, faces[i] * 3);
                Vector3 v1 = RcVec.Create(vertices, faces[i + 1] * 3);
                Vector3 v2 = RcVec.Create(vertices, faces[i + 2] * 3);
                Vector3 e0 = v1 - v0;
                Vector3 e1 = v2 - v0;

                normals[i] = e0.Y * e1.Z - e0.Z * e1.Y;
                normals[i + 1] = e0.Z * e1.X - e0.X * e1.Z;
                normals[i + 2] = e0.X * e1.Y - e0.Y * e1.X;
                float d = MathF.Sqrt(normals[i] * normals[i] + normals[i + 1] * normals[i + 1] + normals[i + 2] * normals[i + 2]);
                if (d > 0)
                {
                    d = 1.0f / d;
                    normals[i] *= d;
                    normals[i + 1] *= d;
                    normals[i + 2] *= d;
                }
            }
        }
    }
}
