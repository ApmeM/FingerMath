namespace FingerMath.Primitives
{
    using System;
    using System.Collections.Generic;

    public struct Polygon : IEquatable<Polygon>
    {
        public List<Vector> points;

        public static Polygon Empty { get; } = new Polygon();

        public bool IsEmpty => this.points.Count == 0;

        public Polygon(params Vector[] points)
        {
            this.points = new List<Vector>(points);
        }

        public Polygon(List<Vector> points)
        {
            this.points = points;
        }

        public bool Contains(float x, float y)
        {
            return this.Contains(new Vector(x, y));
        }

        public bool Contains(Vector p)
        {
            Vector? lastPoint = points[points.Count - 1];
            var crossings = 0;
            foreach (var currentPoint in points)
            {
                try
                {
                    if (lastPoint == null)
                    {
                        continue;
                    }

                    if (lastPoint == p)
                    {
                        return true;
                    }

                    if (((lastPoint.Value.Y <= p.Y && p.Y < currentPoint.Y) || (currentPoint.Y <= p.Y && p.Y < lastPoint.Value.Y))
                            && p.X < ((currentPoint.X - lastPoint.Value.X) / (currentPoint.Y - lastPoint.Value.Y) * (p.Y - lastPoint.Value.Y) + lastPoint.Value.X))
                    {
                        crossings++;
                    }
                }
                finally
                {
                    lastPoint = currentPoint;
                }
            }

            return (crossings % 2 != 0);

        }

        public bool IsIntersectSegment(Segment segment)
        {
            if (segment.From == segment.To)
            {
                return false;
            }

            Vector? lastPoint = points[points.Count - 1];
            foreach (var currentPoint in points)
            {
                try
                {
                    if (lastPoint == null)
                    {
                        continue;
                    }

                    var segment2 = new Segment(currentPoint, lastPoint.Value);
                    if (segment.CheckIntersect(segment2))
                    {
                        return true;
                    }
                }
                finally
                {
                    lastPoint = currentPoint;
                }
            }

            return false;
        }

        public float CalcSquare()
        {
            Vector? lastPoint = points[points.Count - 1];
            float result = 0;
            foreach (var currentPoint in points)
            {
                try
                {
                    if (lastPoint == null)
                    {
                        continue;
                    }

                    result += lastPoint.Value.Cross(currentPoint);
                }
                finally
                {
                    lastPoint = currentPoint;
                }
            }

            return result / 2;
        }

        public Vector CalcCenter()
        {
            if (points.Count == 1)
            {
                return points[0];
            }
            if (points.Count == 2)
            {
                return (points[0] + points[1]) / 2;
            }

            var result = Vector.Zero;
            Vector? lastPoint = points[points.Count - 1];
            var totalAreaX2 = 0f;
            foreach (var currentPoint in points)
            {
                try
                {
                    if (lastPoint == null)
                    {
                        continue;
                    }

                    var areaX2 = lastPoint.Value.Cross(currentPoint);
                    totalAreaX2 += areaX2;

                    result += (lastPoint.Value + currentPoint) * areaX2;
                }
                finally
                {
                    lastPoint = currentPoint;
                }
            }

            result /= (3 * totalAreaX2);

            return result;
        }

        public float CalcRadius()
        {
            var center = CalcCenter();

            var radiusSq = -1f;
            for (int i = 0; i < points.Count; i++)
            {
                var currentRadiusSq = (points[i]-center).LengthQuad;
                if (currentRadiusSq > radiusSq)
                {
                    radiusSq = currentRadiusSq;
                }
            }

            return radiusSq;
        }

        public override string ToString()
        {
            return string.Join(", ", points);
        }

        public override bool Equals(object obj)
        {
            return obj is Polygon d && this.Equals(d);
        }

        public override int GetHashCode()
        {
            int hashCode = -451377473;
            hashCode = hashCode * -1521134295 + points.GetHashCode();
            return hashCode;
        }

        public bool Equals(Polygon d)
        {
            return this.points == d.points;
        }
    }
}