namespace FingerMath
{
    using FingerMath.Collections;
    using FingerMath.Primitives;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     simple ear clipping triangulator. the final triangles will be present in the triangleIndices list
    /// </summary>
    public class Triangulator
    {
        /// <summary>
        ///     Computes a triangle list that fully covers the area enclosed by the given set of points. If points are not CCW,
        ///     pass false for
        ///     the arePointsCCW parameter
        /// </summary>
        /// <param name="points">A list of points that defines an enclosing path.</param>
        /// <param name="arePointsCcw">Flag to know that points are ccw.</param>
        public static List<int> Triangulate(Vector2[] points, bool arePointsCcw = true)
        {
            var count = points.Length;
            var triangleIndices = Pool<List<int>>.Obtain();
            triangleIndices.Clear();
            var triPrev = new int[12];
            var triNext = new int[12];
            if (triNext.Length < count)
                Array.Resize(ref triNext, Math.Max(triNext.Length * 2, count));
            if (triPrev.Length < count)
                Array.Resize(ref triPrev, Math.Max(triPrev.Length * 2, count));

            for (var i = 0; i < count; i++)
            {
                triPrev[i] = i - 1;
                triNext[i] = i + 1;
            }

            triPrev[0] = count - 1;
            triNext[count - 1] = 0;

            // loop breaker for polys that are not triangulatable
            var iterations = 0;

            // start at vert 0
            var index = 0;

            // keep removing verts until just a triangle is left
            while (count > 3 && iterations < 500)
            {
                iterations++;
                // test if current vert is an ear
                var isEar = true;

                var a = points[triPrev[index]];
                var b = points[index];
                var c = points[triNext[index]];

                // an ear must be convex (here counterclockwise)
                if (IsTriangleCcw(a, b, c))
                {
                    // loop over all verts not part of the tentative ear
                    var k = triNext[triNext[index]];
                    do
                    {
                        // if vert k is inside the ear triangle, then this is not an ear
                        if (TestPointTriangle(points[k], a, b, c))
                        {
                            isEar = false;
                            break;
                        }

                        k = triNext[k];
                    }
                    while (k != triPrev[index]);
                }
                else
                {
                    // the ear triangle is clockwise so points[i] is not an ear
                    isEar = false;
                }

                // if current vert is an ear, delete it and visit the previous vert
                if (isEar)
                {
                    // triangle is an ear
                    triangleIndices.Add(triPrev[index]);
                    triangleIndices.Add(index);
                    triangleIndices.Add(triNext[index]);

                    // delete vert by redirecting next and previous links of neighboring verts past it
                    // decrement vertext count
                    triNext[triPrev[index]] = triNext[index];
                    triPrev[triNext[index]] = triPrev[index];
                    count--;

                    // visit the previous vert next
                    index = triPrev[index];
                }
                else
                {
                    // current vert is not an ear. visit the next vert
                    index = triNext[index];
                }
            }

            // output the final triangle
            triangleIndices.Add(triPrev[index]);
            triangleIndices.Add(index);
            triangleIndices.Add(triNext[index]);

            if (!arePointsCcw)
                triangleIndices.Reverse();

            return triangleIndices;
        }

        /// <summary>
        ///     checks if a triangle is CCW or CW
        /// </summary>
        /// <returns><c>true</c>, if triangle ccw was ised, <c>false</c> otherwise.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="center">Center.</param>
        /// <param name="c">C.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTriangleCcw(Vector2 a, Vector2 center, Vector2 c)
        {
            return Cross(center - a, c - center) < 0;
        }

        public static bool TestPointTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            // if point to the right of AB then outside triangle
            if (Cross(point - a, b - a) < 0f)
                return false;

            // if point to the right of BC then outside of triangle
            if (Cross(point - b, c - b) < 0f)
                return false;

            // if point to the right of ca then outside of triangle
            if (Cross(point - c, a - c) < 0f)
                return false;

            // point is in or on triangle
            return true;
        }

        /// <summary>
        ///     compute the 2d pseudo cross product Dot( Perp( u ), v )
        /// </summary>
        /// <param name="u">U.</param>
        /// <param name="v">V.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(Vector2 u, Vector2 v)
        {
            return u.Y * v.X - u.X * v.Y;
        }
    }
}