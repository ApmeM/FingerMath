namespace FingerMath.Tests.Math
{
    using FingerMath.Primitives;
    using NUnit.Framework;

    [TestFixture]
    public class PolygonTests
    {
        [Test]
        public void GetSquare_ForTriangle()
        {
            // Arrange
            var poly = new Polygon(new Vector(0,0), new Vector(2,0), new Vector(0,1));
            // Act
            var square = poly.CalcSquare();
            // Assert
            Assert.AreEqual(1, square);
        }

        [Test]
        public void CalcSquare_For4Points()
        {
            // Arrange
            var poly = new Polygon(new Vector(0,0), new Vector(1,-1), new Vector(2,0), new Vector(1,1));
            // Act
            var square = poly.CalcSquare();
            // Assert
            Assert.AreEqual(2, square);
        }
        
        [Test]
        public void CalcSquare_InvertedDirection()
        {
            // Arrange
            var poly = new Polygon(new Vector(1,1), new Vector(2,0), new Vector(1,-1), new Vector(0,0));
            // Act
            var square = poly.CalcSquare();
            // Assert
            Assert.AreEqual(-2, square);
        }
        
        [Test]
        public void CalcCenter()
        {
            // Arrange
            var poly = new Polygon(new Vector(1,1), new Vector(2,0), new Vector(1,-1), new Vector(0,0));
            // Act
            var square = poly.CalcCenter();
            // Assert
            Assert.AreEqual(Vector.Right, square);
        }
        
        [Test]
        public void CalcRadius()
        {
            // Arrange
            var poly = new Polygon(new Vector(1,1), new Vector(2,0), new Vector(1,-1), new Vector(0,0));
            // Act
            var radiusSq = poly.CalcRadius();
            // Assert
            Assert.AreEqual(1, radiusSq);
        }
    }
}