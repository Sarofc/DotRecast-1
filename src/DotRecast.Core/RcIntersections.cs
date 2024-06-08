/*
Copyright (c) 2009-2010 Mikko Mononen memon@inside.org
recast4j copyright (c) 2021 Piotr Piastucki piotr@jtilia.org
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
using DotRecast.Core.Numerics;
using System.Numerics;

namespace DotRecast.Core
{
    public static class RcIntersections
    {
        public static bool IntersectSegmentTriangle(Vector3 sp, Vector3 sq, Vector3 a, Vector3 b, Vector3 c, out float t)
        {
            t = 0;
            float v, w;
            Vector3 ab = Vector3.Subtract(b, a);
            Vector3 ac = Vector3.Subtract(c, a);
            Vector3 qp = Vector3.Subtract(sp, sq);

            // Compute triangle normal. Can be precalculated or cached if
            // intersecting multiple segments against the same triangle
            Vector3 norm = Vector3.Cross(ab, ac);

            // Compute denominator d. If d <= 0, segment is parallel to or points
            // away from triangle, so exit early
            float d = Vector3.Dot(qp, norm);
            if (d <= 0.0f)
            {
                return false;
            }

            // Compute intersection t value of pq with plane of triangle. A ray
            // intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
            // dividing by d until intersection has been found to pierce triangle
            Vector3 ap = Vector3.Subtract(sp, a);
            t = Vector3.Dot(ap, norm);
            if (t < 0.0f)
            {
                return false;
            }

            if (t > d)
            {
                return false; // For segment; exclude this code line for a ray test
            }

            // Compute barycentric coordinate components and test if within bounds
            Vector3 e = Vector3.Cross(qp, ap);
            v = Vector3.Dot(ac, e);
            if (v < 0.0f || v > d)
            {
                return false;
            }

            w = -Vector3.Dot(ab, e);
            if (w < 0.0f || v + w > d)
            {
                return false;
            }

            // Segment/ray intersects triangle. Perform delayed division
            t /= d;

            return true;
        }

        public static bool IsectSegAABB(Vector3 sp, Vector3 sq, Vector3 amin, Vector3 amax, out float tmin, out float tmax)
        {
            const float EPS = 1e-6f;

            Vector3 d = new Vector3();
            d.X = sq.X - sp.X;
            d.Y = sq.Y - sp.Y;
            d.Z = sq.Z - sp.Z;
            tmin = 0.0f;
            tmax = float.MaxValue;

            for (int i = 0; i < 3; i++)
            {
                if (MathF.Abs(d.Get(i)) < EPS)
                {
                    if (sp.Get(i) < amin.Get(i) || sp.Get(i) > amax.Get(i))
                    {
                        return false;
                    }
                }
                else
                {
                    float ood = 1.0f / d.Get(i);
                    float t1 = (amin.Get(i) - sp.Get(i)) * ood;
                    float t2 = (amax.Get(i) - sp.Get(i)) * ood;

                    if (t1 > t2)
                    {
                        (t1, t2) = (t2, t1);
                    }

                    if (t1 > tmin)
                    {
                        tmin = t1;
                    }

                    if (t2 < tmax)
                    {
                        tmax = t2;
                    }

                    if (tmin > tmax)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}