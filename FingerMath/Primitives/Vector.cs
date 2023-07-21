using System;

namespace FingerMath.Primitives
{
    public struct Vector : IEquatable<Vector>, IComparable<Vector>
    {
        public static Vector Zero = new Vector(0, 0);
        public static Vector Left = new Vector(-1, 0);
        public static Vector Right = new Vector(1, 0);
        public static Vector Up = new Vector(0, -1);
        public static Vector Down = new Vector(0, 1);

        public float X;
        public float Y;

        public Vector(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float ManhattanLength => Math.Abs(this.X) + Math.Abs(this.Y);
        public float Length => (float)Math.Sqrt(this.LengthQuad);

        public float LengthQuad => this.X * this.X + this.Y * this.Y;

        public Vector Normalize() => this / this.Length;

        public float Angle => (float)Math.Atan2(this.Y, this.X);

        public static Vector operator +(Vector a, Vector b)
            => new Vector(a.X + b.X, a.Y + b.Y);

        public static Vector operator -(Vector a, Vector b)
                    => new Vector(a.X - b.X, a.Y - b.Y);

        public static Vector operator -(Vector a)
                    => new Vector(-a.X, -a.Y);

        public static Vector operator *(Vector a, float b)
                    => new Vector(a.X * b, a.Y * b);

        public static Vector operator *(float b, Vector a)
                    => new Vector(a.X * b, a.Y * b);

        public static Vector operator /(Vector a, float b)
                    => new Vector(a.X / b, a.Y / b);

        public static Vector operator /(float b, Vector a)
                    => new Vector(a.X / b, a.Y / b);

        public static bool operator ==(Vector a, Vector b)
                    => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Vector a, Vector b)
                    => !(a == b);

        public float Dot(Vector p2)
        {
            return this.X * p2.X + this.Y * p2.Y;
        }

        // In case points set in CCW order, the value will be positive, otherwice - negative.
        // Method can be used as a dotprod between vectors this and perpendicular to p2
        // The overal result value is equl to doubled triangle size between those 2 points and point (0,0)
        public float Cross(Vector p2)
        {
            return this.X * p2.Y - this.Y * p2.X;
        }

        public Vector Rotate(float angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            return new Vector(cos * this.X - sin * this.Y, sin * this.X + cos * this.Y);
        }

        public float AngleToVector(Vector vec2)
        {
            return (float)Math.Acos((this.X * vec2.X + this.Y * vec2.Y) / (this.Length * vec2.Length));
        }

        public override bool Equals(object obj)
        {
            return obj is Vector vector && this.Equals(vector);
        }

        public override int GetHashCode()
        {
            int hashCode = -1274299002;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public bool Equals(Vector vector)
        {
            return X == vector.X &&
                   Y == vector.Y;
        }

        /// Comparison is done by angle in CCW order from 0 to 2PI. 
        public int CompareTo(Vector other)
        {
            if (this.Y == 0 && other.Y == 0)
            {
                return Math.Sign(Math.Sign(this.X) - Math.Sign(other.X));
            }

            if ((this.Y >= 0) ^ (other.Y >= 0))
            {
                if (this.Y >= 0)
                    return 1;
                else
                    return -1;
            }
            else
            {
                return Math.Sign((other - this).Cross(-this));
            }
        }
    }
}
