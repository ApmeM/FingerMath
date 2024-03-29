﻿using System;

namespace FingerMath.Primitives
{
    public struct Segment
    {
        public Vector From;
        public Vector To;

        public float LengthQuad => (this.From - this.To).LengthQuad;

        public Segment(Vector from, Vector to)
        {
            this.From = from;
            this.To = to;
        }

        public Segment(float fromX, float fromY, float toX, float toY)
        {
            this.From = new Vector(fromX, fromY);
            this.To = new Vector(toX, toY);
        }

        public Vector FindClosestPoint(Vector p)
        {
            var r = this.To - this.From;
            var rlen2 = r.LengthQuad;
            if (rlen2 == 0.0)
                return this.From;

            var t = p - this.From;

            var d = r.Dot(t) / rlen2;
            if (d >= 1.0)
                return this.To;
            if (d <= 0.0)
                return this.From;

            return this.From + r * d;
        }

        public double PointToLineDistSq(Vector fromPoint)
        {
            var triangle = (this.To - this.From).Cross(fromPoint - this.From);
            return triangle * triangle / this.LengthQuad;
        }

        public bool CheckIntersect(Segment seg2)
        {
            var a = this.To - this.From;
            var b = seg2.From - seg2.To;
            var d = seg2.From - this.From;

            var det = a.Cross(b);
            var r = d.Cross(b);
            var s = a.Cross(d);

            if (det > 0)
                return !(r < 0 || r > det || s < 0 || s > det);
            if (det < 0)
                return !(-r < 0 || -r > -det || -s < 0 || -s > -det);
            return false;
        }
    }
}
