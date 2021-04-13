namespace FingerMath.Primitives
{
    using System.Collections.Generic;

    /// <summary>
    ///     houses a series of cubic bezier points and provides helper methods to access the bezier
    /// </summary>
    public class BezierSpline
    {
        private readonly List<Vector2> points = new List<Vector2>();

        private int curveCount;

        /// <summary>
        ///     helper that gets the bezier point index at time t. t is modified in the process to be in the range of the curve
        ///     segment.
        /// </summary>
        /// <returns>The index at time.</returns>
        /// <param name="t">T.</param>
        private int PointIndexAtTime(ref float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = this.points.Count - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * this.curveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }

            return i;
        }

        /// <summary>
        ///     sets a control point taking into account if this is a shared point and adjusting appropriately if it is
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="point">Point.</param>
        public void SetControlPoint(int index, Vector2 point)
        {
            if (index % 3 == 0)
            {
                var delta = point - this.points[index];
                if (index > 0)
                    this.points[index - 1] += delta;

                if (index + 1 < this.points.Count)
                    this.points[index + 1] += delta;
            }

            this.points[index] = point;
        }

        /// <summary>
        ///     gets the point on the bezier at time t
        /// </summary>
        /// <returns>The point at time.</returns>
        /// <param name="t">T.</param>
        public Vector2 GetPointAtTime(float t)
        {
            var i = this.PointIndexAtTime(ref t);
            return Bezier.GetPoint(this.points[i], this.points[i + 1], this.points[i + 2], this.points[i + 3], t);
        }

        /// <summary>
        ///     gets the velocity (first derivative) of the bezier at time t
        /// </summary>
        /// <returns>The velocity at time.</returns>
        /// <param name="t">T.</param>
        public Vector2 GetVelocityAtTime(float t)
        {
            var i = this.PointIndexAtTime(ref t);
            return Bezier.GetFirstDerivative(
                this.points[i],
                this.points[i + 1],
                this.points[i + 2],
                this.points[i + 3],
                t);
        }

        /// <summary>
        ///     gets the direction (normalized first derivative) of the bezier at time t
        /// </summary>
        /// <returns>The direction at time.</returns>
        /// <param name="t">T.</param>
        public Vector2 GetDirectionAtTime(float t)
        {
            return this.GetVelocityAtTime(t).Normalize();
        }

        /// <summary>
        ///     adds a curve to the bezier
        /// </summary>
        public void AddCurve(Vector2 start, Vector2 firstControlPoint, Vector2 secondControlPoint, Vector2 end)
        {
            // we only add the start point if this is the first curve. For all other curves the previous end should equal the start of the new curve.
            if (this.points.Count == 0)
                this.points.Add(start);

            this.points.Add(firstControlPoint);
            this.points.Add(secondControlPoint);
            this.points.Add(end);

            this.curveCount = (this.points.Count - 1) / 3;
        }

        /// <summary>
        ///     resets the bezier removing all points
        /// </summary>
        public void Reset()
        {
            this.points.Clear();
        }

        /// <summary>
        ///     breaks up the spline into totalSegments parts and returns all the points required to draw using lines
        /// </summary>
        /// <returns>The drawing points.</returns>
        /// <param name="totalSegments">Total segments.</param>
        public Vector2[] GetDrawingPoints(int totalSegments)
        {
            var result = new Vector2[totalSegments];
            for (var i = 0; i < totalSegments; i++)
            {
                var t = i / (float)totalSegments;
                result[i] = this.GetPointAtTime(t);
            }

            return result;
        }
    }
}