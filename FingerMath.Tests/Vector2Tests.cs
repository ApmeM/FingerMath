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
    }
}