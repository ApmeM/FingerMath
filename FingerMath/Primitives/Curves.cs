﻿namespace FingerMath.Primitives
{
    using System;
    using System.Collections.Generic;

    public class Lissajou{
        float xFrequency = 2f;
        float yFrequency = 3f;
        float xMagnitude = 1;
        float yMagnitude = 1;
        float phase = 0;

        public Lissajou(
            float xFrequency = 2f,
            float yFrequency = 3f,
            float xMagnitude = 1,
            float yMagnitude = 1,
            float phase = 0)
        {
            this.xFrequency = xFrequency;
            this.yFrequency = yFrequency;
            this.xMagnitude = xMagnitude;
            this.yMagnitude = yMagnitude;
            this.phase = phase;
        }

        public Vector GetPoint(float t)
        {
            var x = (float)Math.Sin(t * xFrequency + phase) * xMagnitude;
            var y = (float)Math.Cos(t * yFrequency) * yMagnitude;

            return new Vector(x, y);
        }
    }

    public class CubicBezier
    {
        public Vector p0 { get; private set; }
        public Vector p1 { get; private set; }
        public Vector p2 { get; private set; }

        public CubicBezier(Vector p0, Vector p1, Vector p2)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
        }

        public Vector GetPoint(float t)
        {
            t = Mathf.Clamp(t, 0, 1);
            var oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
        }

        /// <summary>
        ///     gets the first derivative for a quadratic bezier
        /// </summary>
        public Vector GetFirstDerivative(float t)
        {
            return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
        }
    }

    public class QuadraticBezier
    {
        public Vector p0 { get; private set; }
        public Vector p1 { get; private set; }
        public Vector p2 { get; private set; }
        public Vector p3 { get; private set; }

        public QuadraticBezier(Vector p0, Vector p1, Vector p2, Vector p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public Vector GetPoint(float t)
        {
            t = Mathf.Clamp(t, 0, 1);
            var oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * p0 
                    + 3f * oneMinusT * oneMinusT * t * p1
                    + 3f * oneMinusT * t * t * p2
                    + t * t * t * p3;
        }

        public Vector GetFirstDerivative(float t)
        {
            t = Mathf.Clamp(t, 0, 1);
            var oneMinusT = 1f - t;
            return 3f * oneMinusT * oneMinusT * (p1 - p0)
                   + 6f * oneMinusT * t * (p2 - p1)
                   + 3f * t * t * (p3 - p2);
        }
    }
    
    public class QuadraticBezierSpline
    {
        private readonly List<QuadraticBezier> points = new List<QuadraticBezier>();

        public QuadraticBezierSpline(List<QuadraticBezier> curves)
        {
            for (var i = 1; i < curves.Count; i++)
            {
                if (curves[i].p0 != curves[i-1].p3)
                {
                    throw new Exception($"Curves at point {i-1} and {i} are not connected (end of first not equal to start of second)");
                }
            }

            this.points = curves;
        }

        private QuadraticBezier GetCurveAtTime(ref float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                return points[this.points.Count - 1];
            }

            t = Mathf.Clamp(t, 0, 1) * this.points.Count;
            i = (int)t;
            t -= i;

            return points[i];
        }

        public Vector GetPoint(float t)
        {
            return this.GetCurveAtTime(ref t).GetPoint(t);
        }

        public Vector GetFirstDerivative(float t)
        {
            return this.GetCurveAtTime(ref t).GetFirstDerivative(t);
        }
    }
}