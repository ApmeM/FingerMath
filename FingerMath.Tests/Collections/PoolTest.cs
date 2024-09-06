using FingerMath.Collections;
using NUnit.Framework;

namespace FingerMath.Tests.Collections
{
    [TestFixture]
    public class PoolTest
    {
        private class PoolObj
        {
            public int i;
        }
        private class PoolObjResetable : IPoolable
        {
            public int i;

            public void Reset()
            {
                i = 0;
            }
        }

        [Test]
        public void ReturnObjectManually_NextObtainReturnIt()
        {
            var obj = Pool<PoolObj>.Obtain();
            obj.Data.i = 2;
            Pool<PoolObj>.Return(obj.Data);

            var obj2 = Pool<PoolObj>.Obtain();

            Assert.AreEqual(2, obj2.Data.i);
        }

        [Test]
        public void ReturnObjectByDisposable_NextObtainReturnIt()
        {
            using (var obj = Pool<PoolObj>.Obtain())
            {
                obj.Data.i = 2;
            }

            var obj2 = Pool<PoolObj>.Obtain();

            Assert.AreEqual(2, obj2.Data.i);
        }

        [Test]
        public void ReturnObjectByDisposable_PoolObjectWithReset_ValueResetted()
        {
            using (var obj = Pool<PoolObjResetable>.Obtain())
            {
                obj.Data.i = 2;
            }

            var obj2 = Pool<PoolObjResetable>.Obtain();

            Assert.AreEqual(0, obj2.Data.i);
        }
    }
}
