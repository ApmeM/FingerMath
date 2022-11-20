namespace FingerMath.Tests.Math
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class DfsSortTests
    {
        [Test]
        public void Sort_DependentVariants_Sorted()
        {
            var result = DfsSort.Sort(new List<int>{1,2,3,4}, new Dictionary<int, List<int>>{
                {1, new List<int>{3}},
                {4, new List<int>{3}},
                {3, new List<int>{2}}
            });

            Assert.AreEqual(result.Count(), 3);
            Assert.AreEqual(result[0].First(), 2);
            Assert.AreEqual(result[1].First(), 3);
            Assert.AreEqual(result[2].First(), 1);
            Assert.AreEqual(result[2].Skip(1).First(), 4);
        }
    }
}