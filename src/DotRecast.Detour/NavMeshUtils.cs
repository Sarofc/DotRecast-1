/*
recast4j copyright (c) 2021 Piotr Piastucki piotr@jtilia.org

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
using DotRecast.Core;
using DotRecast.Detour;

namespace DotRecast.Detour
{
    public static class NavMeshUtils
    {
        public static Vector3f[] getNavMeshBounds(NavMesh mesh)
        {
            Vector3f bmin = Vector3f.Of(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            Vector3f bmax = Vector3f.Of(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            for (int t = 0; t < mesh.getMaxTiles(); ++t)
            {
                MeshTile tile = mesh.getTile(t);
                if (tile != null && tile.data != null)
                {
                    for (int i = 0; i < tile.data.verts.Length; i += 3)
                    {
                        bmin[0] = Math.Min(bmin[0], tile.data.verts[i]);
                        bmin[1] = Math.Min(bmin[1], tile.data.verts[i + 1]);
                        bmin[2] = Math.Min(bmin[2], tile.data.verts[i + 2]);
                        bmax[0] = Math.Max(bmax[0], tile.data.verts[i]);
                        bmax[1] = Math.Max(bmax[1], tile.data.verts[i + 1]);
                        bmax[2] = Math.Max(bmax[2], tile.data.verts[i + 2]);
                    }
                }
            }

            return new[] { bmin, bmax };
        }
    }
}