namespace FingerMath.Tests.Collections
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class LookupTest
    {
        [Test]
        public void CountsTest()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 2 },
                { 2, 3 },
                { 1, 3 }
            };

            Assert.AreEqual(2, lookup.Count);
            Assert.AreEqual(1, lookup[2].Count);
            Assert.AreEqual(2, lookup[1].Count);
        }
        

        [Test]
        public void CountNonExistingIndex()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
            };

            CollectionAssert.AreEqual(new List<int>(), lookup[1]);
        }

        [Test]
        public void RemoveLastElement()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 2 },
                { 2, 3 }
            };

            lookup.Remove(2, 3);
            Assert.AreEqual(0, lookup[2].Count);
        }

        [Test]
        public void RemoveFirstElement()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 2 },
                { 1, 3 }
            };

            lookup.Remove(1, 2);
            CollectionAssert.AreEqual(new List<int> { 3 }, lookup[1]);
        }

        [Test]
        public void RemoveTheOnlyElement()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 2 }
            };

            lookup.Remove(1, 2);
            Assert.AreEqual(0, lookup[1].Count);
        }

        [Test]
        public void RemoveWitProperKeyValue()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 2 },
                { 2, 2 },
                { 2, 3 }
            };

            lookup.Remove(2, 2);
            Assert.AreEqual(1, lookup[2].Count);
            Assert.AreEqual(1, lookup[1].Count);
        }

        [Test]
        public void IgnoreDuplicate()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>(true)
            {
                { 1, 2 },
                { 1, 3 },
                { 2, 3 },
                { 1, 2 },
                { 1, 3 }
            };

            Assert.AreEqual(2, lookup.Count);
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, lookup[1]);
            CollectionAssert.AreEqual(new List<int> { 3 }, lookup[2]);
        }

        [Test]
        public void CheckLookupOrder()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>(false)
            {
                { 1, 1 },
                { 1, 3 },
                { 1, 1 },
                { 1, 5 }
            };

            CollectionAssert.AreEqual(new List<int> { 1, 3, 1, 5 }, lookup[1]);
        }

        [Test]
        public void AddToLookupEnumerator()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 1 }
            };
            lookup[1].Add(10);

            CollectionAssert.AreEqual(new List<int> { 1, 10 }, lookup[1]);
        }

        [Test]
        public void AddToNonExistingLookupEnumerator()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 1 }
            };
            lookup[2].Add(10);

            CollectionAssert.AreEqual(new List<int> { 10 }, lookup[2]);
            CollectionAssert.AreEqual(new List<int> { 1 }, lookup[1]);
        }

        [Test]
        public void ClearLookupEnumerator()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 1 }
            };
            lookup[1].Clear();

            CollectionAssert.AreEqual(new List<int>(), lookup[1]);
        }

        [Test]
        public void RemoveExistingFromLookupEnumerator()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 1 }
            };
            lookup[1].Remove(1);

            CollectionAssert.AreEqual(new List<int>(), lookup[1]);
        }

        [Test]
        public void RemoveNotExistingFromGroupEnumerator()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 1 }
            };
            lookup.Remove(1, 2);

            CollectionAssert.AreEqual(new List<int> { 1 }, lookup[1]);
        }

        [Test]
        public void RemoveNotExistingFromIndexedEnumerator()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 1 }
            };
            lookup[1].Remove(2);

            CollectionAssert.AreEqual(new List<int> { 1 }, lookup[1]);
        }

        [Test]
        public void RemoveFromEmptyLookup()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
            };
            lookup[1].Remove(2);

            CollectionAssert.AreEqual(new List<int>(), lookup[1]);
        }

        [Test]
        public void EditDuringGroupEnumerating()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 10 },
                { 1, 20 }
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                foreach (var i in lookup)
                {
                    i.Add(i.Count);
                }
            });
            CollectionAssert.AreEqual(new List<int> { 10, 20, 2 }, lookup[1]);
        }

        [Test]
        public void EditDuringEnumerating()
        {
            var lookup = new FingerMath.Collections.Lookup<int, int>
            {
                { 1, 10 },
                { 1, 20 }
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                foreach (var i in lookup[1])
                {
                    lookup.Add(1, 2);
                }
            });
            CollectionAssert.AreEqual(new List<int> { 10, 20, 2 }, lookup[1]);
        }
    }
}
