namespace FingerMath.Tests.Math
{
    using FingerMath.Primitives;
    using NUnit.Framework;

    [TestFixture]
    public class Vector2Tests
    {
        [Test]
        public void AngleTest()
        {
            var v1 = new Vector2(10, 10);

            var angle = v1.Angle;

            Assert.AreEqual((float)System.Math.PI/4, angle);
        }

        [Test]
        public void RotateTest()
        {
            var v1 = new Vector2(10, 10);

            var result = v1.Rotate((float)System.Math.PI);

            Assert.AreEqual(-10, result.X, 0.00001f);
            Assert.AreEqual(-10, result.Y, 0.00001f);
        }

        [Test]
        public void AngleToVectorTest()
        {
            var v1 = new Vector2(10, 10);
            var v2 = new Vector2(-10, 10);

            var result = v1.AngleToVector(v2);

            Assert.AreEqual((float)System.Math.PI / 2, result, 0.00001f);
        }
    }
}