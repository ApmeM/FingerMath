namespace FingerMath.Tests.Math
{
    using FingerMath.Matching;
    using NUnit.Framework;

    [TestFixture]
    public class MatchingTests
    {
        private Matcher matcher;

        public class Required { }
        public class Required2 { }
        public class Prohibited { }
        public class Prohibited2 { }
        public class RequiredVariant { }
        public class RequiredVariant2 { }

        [SetUp]
        public void Setup()
        {
            this.matcher = Matcher.Empty()
                .All(TypeBitSetManager.GetBitsFromTypes(new[] { typeof(Required), typeof(Required2) }))
                .Exclude(TypeBitSetManager.GetBitsFromTypes(new[] { typeof(Prohibited), typeof(Prohibited2) }))
                .One(TypeBitSetManager.GetBitsFromTypes(new[] { typeof(RequiredVariant), typeof(RequiredVariant2) }));

        }

        [Test]
        public void IsMatched_AllRequiredComponentsExists_Matched()
        {
            var isMatched = matcher.IsMatched(TypeBitSetManager.GetBitsFromTypes(new[]{
                typeof(Required), typeof(Required2), typeof(RequiredVariant)
            }));

            Assert.IsTrue(isMatched);
        }

        [Test]
        public void IsMatched_OneRequiredComponentAbsent_NotMatched()
        {
            var isMatched = matcher.IsMatched(TypeBitSetManager.GetBitsFromTypes(new[]{
                typeof(Required), typeof(RequiredVariant)
            }));

            Assert.IsFalse(isMatched);
        }

        [Test]
        public void IsMatched_AnyRequiredVariantsAbsent_NotMatched()
        {
            var isMatched = matcher.IsMatched(TypeBitSetManager.GetBitsFromTypes(new[]{
                typeof(Required), typeof(Required2)
            }));

            Assert.IsFalse(isMatched);
        }

        [Test]
        public void IsMatched_ProhibitedVariantExist_NotMatched()
        {
            var isMatched = matcher.IsMatched(TypeBitSetManager.GetBitsFromTypes(new[]{
                typeof(Required), typeof(Required2), typeof(RequiredVariant), typeof(Prohibited)
            }));

            Assert.IsFalse(isMatched);
        }

        [Test]
        public void IsMatched_EmptyBitset_NotMatched()
        {
            var isMatched = matcher.IsMatched(new BitSet());

            Assert.IsFalse(isMatched);
        }

        [Test]
        public void IsMatched_EmptyMatcherWithRandomComponents_Matched()
        {
            var isMatched = Matcher.Empty().IsMatched(TypeBitSetManager.GetBitsFromTypes(new[]{
                typeof(Required), typeof(Prohibited)
            }));

            Assert.IsTrue(isMatched);
        }
    }
}