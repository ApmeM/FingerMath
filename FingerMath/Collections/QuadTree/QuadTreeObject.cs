﻿namespace FingerMath.Collections.QuadTree
{
    /// <summary>
    ///     Used internally to attach an Owner to each object stored in the QuadTree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class QuadTreeObject<T>
        where T : IQuadTreeStorable
    {
        /// <summary>
        ///     The wrapped data value
        /// </summary>
        public T Data;

        /// <summary>
        ///     The QuadTreeNode that owns this object
        /// </summary>
        internal QuadTreeNode<T> Owner;

        /// <summary>
        ///     Wraps the data value
        /// </summary>
        /// <param name="data">The data value to wrap</param>
        public QuadTreeObject(T data)
        {
            this.Data = data;
        }
    }
}