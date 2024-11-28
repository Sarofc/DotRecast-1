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

using System.Numerics;
using System.Runtime.InteropServices;

namespace DotRecast.Detour.TileCache
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct DtObstacleUnion
    {
        [FieldOffset(0)]
        public DtObstacleCylinder cylinder;
        [FieldOffset(0)]
        public DtObstacleBox box;
        [FieldOffset(0)]
        public DtObstacleOrientedBox orientedBox;
    }

    public struct DtObstacleCylinder
    {
        public Vector3 pos;
        public float radius;
        public float height;
    }

    public struct DtObstacleOrientedBox
    {
        public Vector3 center;
        public Vector3 extents;
        public Vector2 rotAux; // { Cos(0.5f*angle)*Sin(-0.5f*angle); Cos(0.5f*angle)*Cos(0.5f*angle) - 0.5 } 
    }

    public struct DtObstacleBox
    {
        public Vector3 bmin;
        public Vector3 bmax;
    }
}