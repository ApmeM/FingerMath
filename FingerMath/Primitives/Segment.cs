using System;

namespace FingerMath.Primitives
{
    public struct Segment
    {

        public Vector2 From;
        public Vector2 To;


        public Segment(Vector2 from, Vector2 to)
        {
            this.From = from;
            this.To = to;
        }

        public Segment(float fromX, float fromY, float toX, float toY)
        {
            this.From = new Vector2(fromX, fromY);
            this.To = new Vector2(toX, toY);
        }

        public Vector2 FindClosestPoint(Vector2 p)
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

        public bool CheckIntersect(Segment seg2)
        {
            var a = this.To - this.From;
            var b = seg2.From - seg2.To;
            var d = seg2.From - this.From;

            var det = a.X * b.Y - a.Y * b.X;

            if (det == 0)
                return false;

            var r = (d.X * b.Y - d.Y * b.X) / det;
            var s = (a.X * d.Y - a.Y * d.X) / det;

            return !(r < 0 || r > 1 || s < 0 || s > 1);
        }
    }
}
