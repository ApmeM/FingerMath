using System;

namespace FingerMath.Primitives
{
    public struct Vector2
    {

        public float X;
        public float Y;


        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2(Point from, Point to)
        {
            this.X = to.X - from.X;
            this.Y = to.Y - from.Y;
        }

        public float Length => (float)Math.Sqrt(this.LengthQuad);

        public float LengthQuad => this.X * this.X + this.Y * this.Y;

        public Vector2 Normalize() => this / this.Length;

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

        
        public static float AngleBetweenVectors(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }

        /// <summary>
        ///     helper for moving a value around in a circle.
        /// </summary>
        public static Vector2 RotateAround(Vector2 position, float speed, float elapsedSeconds)
        {
            var time = elapsedSeconds * speed;

            var x = (float)Math.Cos(time);
            var y = (float)Math.Sin(time);

            return new Vector2(position.X + x, position.Y + y);
        }

        /// <summary>
        ///     the rotation is relative to the current position not the total rotation. For example, if you are currently at PI/2
        ///     degrees and
        ///     want to rotate to 3PI/4 degrees, you would use an angle of PI/4, not 3PI/4.
        /// </summary>
        public static Vector2 RotateAround(Vector2 point, Vector2 center, float angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);
            var rotatedX = cos * (point.X - center.X) - sin * (point.Y - center.Y) + center.X;
            var rotatedY = sin * (point.X - center.X) + cos * (point.Y - center.Y) + center.Y;

            return new Vector2(rotatedX, rotatedY);
        }

        /// <summary>
        ///     gets a point on the circumference of the circle given its center, radius and angle. 0 degrees is 3 o'clock.
        /// </summary>
        /// <returns>The on circle.</returns>
        /// <param name="circleCenter">Circle center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="angleInDegrees">Angle in degrees.</param>
        public static Vector2 PointOnCircle(Vector2 circleCenter, float radius, float angle)
        {
            return new Vector2
            {
                X = (float)Math.Cos(angle) * radius + circleCenter.X,
                Y = (float)Math.Sin(angle) * radius + circleCenter.Y
            };
        }

        /// <summary>
        ///     lissajou curve
        /// </summary>
        public static Vector2 Lissajou(
            float elapsedSeconds,
            float xFrequency = 2f,
            float yFrequency = 3f,
            float xMagnitude = 1,
            float yMagnitude = 1,
            float phase = 0)
        {
            var x = (float)Math.Sin(elapsedSeconds * xFrequency + phase) * xMagnitude;
            var y = (float)Math.Cos(elapsedSeconds * yFrequency) * yMagnitude;

            return new Vector2(x, y);
        }

        /// <summary>
        ///     damped version of a lissajou curve with oscillation between 0 and max magnitude over time. Damping should be
        ///     between 0 and 1 for best
        ///     results. oscillationInterval is the time in seconds for half of the animation loop to complete.
        /// </summary>
        public static Vector2 LissajouDamped(
            float elapsedSeconds,
            float xFrequency = 2f,
            float yFrequency = 3f,
            float xMagnitude = 1,
            float yMagnitude = 1,
            float phase = 0.5f,
            float damping = 0f,
            float oscillationInterval = 5f)
        {
            var wrappedTime = Mathf.PingPong(elapsedSeconds, oscillationInterval);
            var damped = (float)Math.Pow(Math.E, -damping * wrappedTime);

            var x = damped * (float)Math.Sin(elapsedSeconds * xFrequency + phase) * xMagnitude;
            var y = damped * (float)Math.Cos(elapsedSeconds * yFrequency) * yMagnitude;

            return new Vector2(x, y);
        }

    }
}
