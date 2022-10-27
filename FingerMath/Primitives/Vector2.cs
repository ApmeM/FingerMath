using System;

namespace FingerMath.Primitives
{
    public struct VectorF
    {
        public static VectorF Zero = new VectorF(0, 0);
        public static VectorF Left = new VectorF(-1, 0);
        public static VectorF Right = new VectorF(1, 0);
        public static VectorF Up = new VectorF(0, -1);
        public static VectorF Down = new VectorF(0, 1);

        public float X;
        public float Y;

        public VectorF(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float Length => (float)Math.Sqrt(this.LengthQuad);

        public float LengthQuad => this.X * this.X + this.Y * this.Y;

        public VectorF Normalize() => this / this.Length;
        
        public float Angle => (float)Math.Atan2(this.Y, this.X);

        public static VectorF operator +(VectorF a, VectorF b)
            => new VectorF(a.X + b.X, a.Y + b.Y);

        public static VectorF operator -(VectorF a, VectorF b)
                    => new VectorF(a.X - b.X, a.Y - b.Y);

        public static VectorF operator *(VectorF a, float b)
                    => new VectorF(a.X * b, a.Y * b);

        public static VectorF operator *(float b, VectorF a)
                    => new VectorF(a.X * b, a.Y * b);

        public static VectorF operator /(VectorF a, float b)
                    => new VectorF(a.X / b, a.Y / b);

        public static VectorF operator /(float b, VectorF a)
                    => new VectorF(a.X / b, a.Y / b);

        public static bool operator ==(VectorF a, VectorF b)
                    => a.X == b.X && a.Y == b.Y ;

        public static bool operator !=(VectorF a, VectorF b)
                    => !(a == b);

        public float Dot(VectorF p2)
        {
            return this.X * p2.X + this.Y * p2.Y;
        }

        public float Cross(VectorF p2)
        {
            return this.X * p2.Y - this.Y * p2.X;
        }
        
        public VectorF Rotate(float angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            return new VectorF(cos * this.X - sin * this.Y, sin * this.X +cos * this.Y);
        }
       
        public float AngleToVector(VectorF vec2)
        {
            return (float)Math.Acos((this.X * vec2.X + this.Y * vec2.Y) / (this.Length * vec2.Length));
        }
    }
}
