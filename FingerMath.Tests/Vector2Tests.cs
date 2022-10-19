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
        public void ClosestPointToSegment_OutsideOfSegment_Test()
        {
            var segP1 = new Vector2(10, 10);
            var segP2 = new Vector2(0, 0);
            
            var p = new Vector2(20, 20);

            var result = Vector2.ClosestPointToSegment(segP1, segP2, p);

            Assert.AreEqual(10, result.X);
            Assert.AreEqual(10, result.Y);
        }
        
        [Test]
        public void ClosestPointToSegment_InsideSegment_Test()
        {
            var segP1 = new Vector2(10, 10);
            var segP2 = new Vector2(0, 0);
            
            var p = new Vector2(5, 5);

            var result = Vector2.ClosestPointToSegment(segP1, segP2, p);

            Assert.AreEqual(5, result.X);
            Assert.AreEqual(5, result.Y);
        }
        
        [Test]
        public void ClosestPointToSegment_NotOnSegment_Test()
        {
            var segP1 = new Vector2(10, 10);
            var segP2 = new Vector2(0, 0);
            
            var p = new Vector2(4, 6);

            var result = Vector2.ClosestPointToSegment(segP1, segP2, p);

            Assert.AreEqual(5, result.X);
            Assert.AreEqual(5, result.Y);
        }
        
        [Test]
        public void CheckSegsIntersect_True_Test()
        {
            var start1 = new Vector2(10, 10);
            var end1 = new Vector2(0, 0);
            
            var start2 = new Vector2(10, 0);
            var end2 = new Vector2(0, 10);

            var result = Vector2.CheckSegsIntersect(start1, end1, start2, end2);

            Assert.AreEqual(true, result);
        }
        
        [Test]
        public void CheckSegsIntersect_False_Test()
        {
            var start1 = new Vector2(10, 10);
            var end1 = new Vector2(0, 0);
            
            var start2 = new Vector2(10, 0);
            var end2 = new Vector2(6, 4);

            var result = Vector2.CheckSegsIntersect(start1, end1, start2, end2);

            Assert.AreEqual(false, result);
        }
    }
}