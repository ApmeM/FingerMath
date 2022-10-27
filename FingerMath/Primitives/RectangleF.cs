namespace FingerMath.Primitives
{
    using System;

    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public static RectangleF Empty { get; } = new RectangleF();
        public static RectangleF One { get; } = new RectangleF(0, 0, 1, 1);

        public float Left => this.X;
        public float Right => this.X + this.Width;
        public float Top => this.Y;
        public float Bottom => this.Y + this.Height;

        public bool IsEmpty => this.Width == 0 && this.Height == 0 && this.X == 0 && this.Y == 0;

        public VectorF Location
        {
            get => new VectorF(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public VectorF Size
        {
            get => new VectorF(this.Width, this.Height);
            set
            {
                this.Width = value.X;
                this.Height = value.Y;
            }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public RectangleF(VectorF location, VectorF size)
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

        public bool Contains(VectorF value)
        {
            return this.Contains(value.X, value.Y);
        }

        public bool Contains(RectangleF value)
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

        public bool IsIntersects(RectangleF value)
        {
            return value.Left < this.Right && this.Left < value.Right && value.Top < this.Bottom
                   && this.Top < value.Bottom;
        }

        public RectangleF Intersect(RectangleF value2)
        {
            if (!this.IsIntersects(value2))
            {
                return new RectangleF(0, 0, 0, 0);
            }

            var rightSide = Math.Min(this.X + this.Width, value2.X + value2.Width);
            var leftSide = Math.Max(this.X, value2.X);
            var topSide = Math.Max(this.Y, value2.Y);
            var bottomSide = Math.Min(this.Y + this.Height, value2.Y + value2.Height);
            return new RectangleF(leftSide, topSide, rightSide - leftSide, bottomSide - topSide);
        }

        public RectangleF Union(RectangleF value2)
        {
            var x = Math.Min(this.X, value2.X);
            var y = Math.Min(this.Y, value2.Y);
            return new RectangleF(
                x,
                y,
                Math.Max(this.Right, value2.Right) - x,
                Math.Max(this.Bottom, value2.Bottom) - y);
        }

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !(a == b);
        }

        public VectorF GetIntersectPointOnBorder(VectorF position, VectorF direction)
        {
            var p1 = new VectorF(this.Left, position.Y + direction.Y * (this.Left - position.X) / direction.X);
            var p2 = new VectorF(this.Right, position.Y + direction.Y * (this.Right - position.X) / direction.X);
            var p3 = new VectorF(position.X + direction.X * (this.Top - position.Y) / direction.Y, this.Top);
            var p4 = new VectorF(position.X + direction.X * (this.Bottom - position.Y) / direction.Y, this.Bottom);

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
    }
}