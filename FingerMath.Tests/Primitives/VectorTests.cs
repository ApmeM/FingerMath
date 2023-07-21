namespace FingerMath.Tests.Math
{
    using FingerMath.Primitives;
    using NUnit.Framework;

    [TestFixture]
    public class VectorTests
    {
        
        [Test]
        public void DoubledTriangleSquareBy3Dots_CW()
        {
            var result = Vector.Down.Cross(Vector.Right);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void DoubledTriangleSquareBy3Dots_CCW()
        {
            var result = Vector.Right.Cross(Vector.Down);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void AngleTest()
        {
            var v1 = new Vector(10, 10);

            var angle = v1.Angle;

            Assert.AreEqual((float)System.Math.PI/4, angle);
        }

        [Test]
        public void RotateTest()
        {
            var v1 = new Vector(10, 10);

            var result = v1.Rotate((float)System.Math.PI);

            Assert.AreEqual(-10, result.X, 0.00001f);
            Assert.AreEqual(-10, result.Y, 0.00001f);
        }

        [Test]
        public void AngleToVectorTest()
        {
            var v1 = new Vector(10, 10);
            var v2 = new Vector(-10, 10);

            var result = v1.AngleToVector(v2);

            Assert.AreEqual((float)System.Math.PI / 2, result, 0.00001f);
        }
 
        [Test]
        public void CompareVectorsTest_EqualZeroVectors()
        {
            Assert.AreEqual(0, Vector.Zero.CompareTo(Vector.Zero));
            Assert.AreEqual(1, Vector.Zero.CompareTo(Vector.Left));
            Assert.AreEqual(-1, Vector.Left.CompareTo(Vector.Zero));
        }

        [Test]
        public void CompareVectorsTest_ZeroY_CompareDirectionOfX()
        {
            Assert.AreEqual(-1, Vector.Left.CompareTo(Vector.Right));
            Assert.AreEqual(1, Vector.Right.CompareTo(Vector.Left));
       }

        [Test]
        public void CompareVectorsTest_VectorsInDifferentHalfSphere_TopIsFirst()
        {
            Assert.AreEqual(-1, new Vector(-31, -55).CompareTo(new Vector(2, 25)));
            Assert.AreEqual(1, new Vector(-34, 5).CompareTo(new Vector(3, -35)));
        }

        [Test]
        public void CompareVectorsTest_VectorsInSameDirection_Equal()
        {
            Assert.AreEqual(0, new Vector(10, 10).CompareTo(new Vector(9, 9)));
            Assert.AreEqual(0, new Vector(-10, -10).CompareTo(new Vector(-12, -12)));
            Assert.AreEqual(0, new Vector(-30, 0).CompareTo( new Vector(-25, 0)));
            Assert.AreEqual(0, new Vector(5, 0).CompareTo(new Vector(10, 0)));
            Assert.AreEqual(0, new Vector(0, -30).CompareTo(new Vector(0, -25)));
            Assert.AreEqual(0, new Vector(0, 5).CompareTo(new Vector(0, 10)));
        }

        [Test]
        public void CompareVectorsTest_VectorsTopHalfSphere_RightIsFirst()
        {
            Assert.AreEqual(1, new Vector(10, 0).CompareTo(new Vector(11, 1)));
            Assert.AreEqual(-1, new Vector(2, 3).CompareTo(new Vector(1, 1)));
        }

        [Test]
        public void CompareVectorsTest_VectorsBottomHalfSphere_LeftIsFirst()
        {
            Assert.AreEqual(1, new Vector(10, -50).CompareTo(new Vector(11, -20)));
            Assert.AreEqual(-1, new Vector(10, -50).CompareTo(new Vector(-11, -20)));
        }

        [Test]
        public void CompareVectorsTest_OneVectorDirectedToZero()
        {
            Assert.AreEqual(1, new Vector(-2, 0).CompareTo(new Vector(10, -18)));
            Assert.AreEqual(-1, new Vector(10, -18).CompareTo(new Vector(-2, 0)));
        }   
    }
}