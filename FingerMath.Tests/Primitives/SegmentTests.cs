namespace FingerMath.Tests.Math
{
    using FingerMath.Primitives;
    using NUnit.Framework;

    [TestFixture]
    public class SegmentTests
    {
        [Test]
        public void ClosestPointToSegment_OutsideOfSegment_Test()
        {
            var seg = new Segment(10, 10, 0, 0);
            
            var p = new Vector(20, 20);

            var result = seg.FindClosestPoint(p);

            Assert.AreEqual(10, result.X);
            Assert.AreEqual(10, result.Y);
        }
        
        [Test]
        public void ClosestPointToSegment_InsideSegment_Test()
        {
            var seg = new Segment(10, 10, 0, 0);
            
            var p = new Vector(5, 5);

            var result = seg.FindClosestPoint(p);

            Assert.AreEqual(5, result.X);
            Assert.AreEqual(5, result.Y);
        }
        
        [Test]
        public void ClosestPointToSegment_NotOnSegment_Test()
        {
            var seg = new Segment(10, 10, 0, 0);
            
            var p = new Vector(4, 6);

            var result = seg.FindClosestPoint(p);

            Assert.AreEqual(5, result.X);
            Assert.AreEqual(5, result.Y);
        }
        
        [Test]
        public void CheckIntersect_IntersectsInTheMiddle()
        {
            var seg = new Segment(10, 10, 0, 0);
            var seg2 = new Segment(10, 0, 0, 10);
            
            var result = seg.CheckIntersect(seg2);

            Assert.AreEqual(true, result);
        }
        
        [Test]
        public void CheckIntersect_DoesNotIntersect()
        {
            var seg = new Segment(10, 10, 0, 0);
            var seg2 = new Segment(10, 0, 6, 4);
            
            var result = seg.CheckIntersect(seg2);

            Assert.AreEqual(false, result);
        }
         
        [Test]
        public void CheckIntersect_IntersectOnEndDots()
        {
            var seg = new Segment(10, 10, 0, 0);
            var seg2 = new Segment(0, 0, 6, 4);
            
            var result = seg.CheckIntersect(seg2);

            Assert.AreEqual(true, result);
        }
    }
}