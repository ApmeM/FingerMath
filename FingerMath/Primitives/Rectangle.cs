namespace FingerMath.Primitives
{
    using System;

    public struct Rectangle : IEquatable<Rectangle>
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public static Rectangle Empty { get; } = new Rectangle();
        public static Rectangle One { get; } = new Rectangle(0, 0, 1, 1);

        public float Left => this.X;
        public float Right => this.X + this.Width;
        public float Top => this.Y;
        public float Bottom => this.Y + this.Height;

        public bool IsEmpty => this.Width == 0 && this.Height == 0 && this.X == 0 && this.Y == 0;

        public Vector Location
        {
            get => new Vector(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Vector Size
        {
            get => new Vector(this.Width, this.Height);
            set
            {
                this.Width = value.X;
                this.Height = value.Y;
            }
        }

        public Rectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public Rectangle(Vector location, Vector size)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

        public bool Contains(float x, float y)
        {
            return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
        }

        public bool Contains(Vector value)
        {
            return this.Contains(value.X, value.Y);
        }

        public bool Contains(Rectangle value)
        {
            return this.Contains(value.X, value.Y) &&
            this.Contains(value.X + value.Width, value.Y + value.Height);
        }

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            this.X -= horizontalAmount;
            this.Y -= verticalAmount;
            this.Width += horizontalAmount * 2;
            this.Height += verticalAmount * 2;
        }

        public bool IsIntersects(Rectangle value)
        {
            return value.Left < this.Right && this.Left < value.Right && value.Top < this.Bottom
                   && this.Top < value.Bottom;
        }

        public Rectangle Intersect(Rectangle value2)
        {
            if (!this.IsIntersects(value2))
            {
                return new Rectangle(0, 0, 0, 0);
            }

            var rightSide = Math.Min(this.X + this.Width, value2.X + value2.Width);
            var leftSide = Math.Max(this.X, value2.X);
            var topSide = Math.Max(this.Y, value2.Y);
            var bottomSide = Math.Min(this.Y + this.Height, value2.Y + value2.Height);
            return new Rectangle(leftSide, topSide, rightSide - leftSide, bottomSide - topSide);
        }

        public Rectangle Union(Rectangle value2)
        {
            var x = Math.Min(this.X, value2.X);
            var y = Math.Min(this.Y, value2.Y);
            return new Rectangle(
                x,
                y,
                Math.Max(this.Right, value2.Right) - x,
                Math.Max(this.Bottom, value2.Bottom) - y);
        }

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !(a == b);
        }

        public Vector GetIntersectPointOnBorder(Vector position, Vector direction)
        {
            var p1 = new Vector(this.Left, position.Y + direction.Y * (this.Left - position.X) / direction.X);
            var p2 = new Vector(this.Right, position.Y + direction.Y * (this.Right - position.X) / direction.X);
            var p3 = new Vector(position.X + direction.X * (this.Top - position.Y) / direction.Y, this.Top);
            var p4 = new Vector(position.X + direction.X * (this.Bottom - position.Y) / direction.Y, this.Bottom);

            if (direction.X < 0)
            {
                if (direction.Y < 0)
                {
                    var dist1 = (position - p1).LengthQuad;
                    var dist2 = (position - p3).LengthQuad;
                    return dist1 > dist2 ? p3 : p1;
                }
                else
                {
                    var dist1 = (position - p1).LengthQuad;
                    var dist2 = (position - p4).LengthQuad;
                    return dist1 > dist2 ? p4 : p1;
                }
            }
            else
            {
                if (direction.Y < 0)
                {
                    var dist1 = (position - p2).LengthQuad;
                    var dist2 = (position - p3).LengthQuad;
                    return dist1 > dist2 ? p3 : p2;
                }
                else
                {
                    var dist1 = (position - p2).LengthQuad;
                    var dist2 = (position - p4).LengthQuad;
                    return dist1 > dist2 ? p4 : p2;
                }
            }
        }

        public override string ToString()
        {
            return $"X:{this.X}, Y:{this.Y}, Width: {this.Width}, Height: {this.Height}";
        }

        public override bool Equals(object obj)
        {
            return obj is Rectangle d && this.Equals(d);
        }

        public override int GetHashCode()
        {
            int hashCode = -451377473;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + Left.GetHashCode();
            hashCode = hashCode * -1521134295 + Right.GetHashCode();
            hashCode = hashCode * -1521134295 + Top.GetHashCode();
            hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
            hashCode = hashCode * -1521134295 + IsEmpty.GetHashCode();
            hashCode = hashCode * -1521134295 + Location.GetHashCode();
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            return hashCode;
        }

        public bool Equals(Rectangle d)
        {
            return X == d.X &&
                   Y == d.Y &&
                   Width == d.Width &&
                   Height == d.Height;
        }
    }
}