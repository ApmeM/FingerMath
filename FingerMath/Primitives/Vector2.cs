using System;

namespace FingerMath.Primitives
{
    public struct Vector2
    {
        public static Vector2 Zero = new Vector2(0, 0);
        public static Vector2 Left = new Vector2(-1, 0);
        public static Vector2 Right = new Vector2(1, 0);
        public static Vector2 Up = new Vector2(0, -1);
        public static Vector2 Down = new Vector2(0, 1);

        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float Length => (float)Math.Sqrt(this.LengthQuad);

        public float LengthQuad => this.X * this.X + this.Y * this.Y;

        public Vector2 Normalize() => this / this.Length;
        
        public float Angle => (float)Math.Atan2(this.Y, this.X);

        public static Vector2 operator +(Vector2 a, Vector2 b)
            => new Vector2(a.X + b.X, a.Y + b.Y);

        public static Vector2 operator -(Vector2 a, Vector2 b)
                    => new Vector2(a.X - b.X, a.Y - b.Y);

        public static Vector2 operator *(Vector2 a, float b)
                    => new Vector2(a.X * b, a.Y * b);

        public static Vector2 operator *(float b, Vector2 a)
                    => new Vector2(a.X * b, a.Y * b);

        public static Vector2 operator /(Vector2 a, float b)
                    => new Vector2(a.X / b, a.Y / b);

        public static Vector2 operator /(float b, Vector2 a)
                    => new Vector2(a.X / b, a.Y / b);

        public static bool operator ==(Vector2 a, Vector2 b)
                    => a.X == b.X && a.Y == b.Y ;

        public static bool operator !=(Vector2 a, Vector2 b)
                    => !(a == b);

        public float Dot(Vector2 p2)
        {
            return this.X * p2.X + this.Y * p2.Y;
        }

        public float Cross(Vector2 p2)
        {
            return this.X * p2.Y - this.Y * p2.X;
        }
        
        public Vector2 Rotate(float angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            return new Vector2(cos * this.X - sin * this.Y, sin * this.X +cos * this.Y);
        }
       
        public float AngleToVector(Vector2 vec2)
        {
            return (float)Math.Acos((this.X * vec2.X + this.Y * vec2.Y) / (this.Length * vec2.Length));
        }
    }
}
