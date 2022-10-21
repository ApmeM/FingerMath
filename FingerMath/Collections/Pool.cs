namespace FingerMath.Collections
{
    using System.Collections.Generic;

    public static class Pool<T> where T : new()
    {
        private static readonly Queue<T> objectQueue = new Queue<T>();

        public static void ClearCache()
        {
            objectQueue.Clear();
        }

        public static T Obtain()
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