namespace FingerMath.Collections
{
    using System;
    using System.Collections.Generic;

    public struct Poolable<T> : IDisposable where T : new()
    {
        public T Data;

        public void Dispose()
        {
            Pool<T>.Return(Data);
        }

        public static implicit operator T(Poolable<T> a)
        {
            return a.Data;
        }

        public static implicit operator Poolable<T>(T a)
        {
            return new Poolable<T>
            {
                Data = a
            };
        }
    }

    public static class Pool<T> where T : new()
    {
        private static readonly Queue<T> objectQueue = new Queue<T>();

        public static void ClearCache()
        {
            objectQueue.Clear();
        }

        public static Poolable<T> Obtain()
        {
            if (objectQueue.Count > 0)
                return objectQueue.Dequeue();

            return new T();
        }

        public static void Return(T obj)
        {
            objectQueue.Enqueue(obj);
            (obj as IPoolable)?.Reset();
        }
    }

    public interface IPoolable
    {
        void Reset();
    }
}