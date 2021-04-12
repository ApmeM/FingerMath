namespace FingerMath.Tests.Math
{
    using FingerMath.Primitives;
    using NUnit.Framework;

    [TestFixture]
    public class RectangleFTests
    {
        [Test]
        public void Inflate_ExtendSize()
        {
            var rect = new RectangleF(10, 10, 10, 10);
            var anotherRect = rect;
            anotherRect.Inflate(10, 10);

            Assert.AreEqual(rect.X - 10, anotherRect.X);
            Assert.AreEqual(rect.Y - 10, anotherRect.Y);
            Assert.AreEqual(rect.Width + 20, anotherRect.Width);
            Assert.AreEqual(rect.Height + 20, anotherRect.Height);
        }
    }
}