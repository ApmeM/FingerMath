namespace FingerMath.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     Sourced from: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    ///     An implementation of a min-Priority Queue using a heap.  Has O(1) .Contains()!
    ///     See https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp/wiki/Getting-Started for more information
    /// </summary>
    /// <typeparam name="T">The values in the queue.  Must extend the FastPriorityQueueNode class</typeparam>
    public sealed class PriorityQueue<T> : IEnumerable where T : PriorityQueueNode
    {
        private T[] nodes;

        private long numNodesEverEnqueued;

        /// <summary>
        ///     Instantiate a new Priority Queue
        /// </summary>
        /// <param name="maxNodes">The max nodes ever allowed to be enqueued (going over this will cause undefined behavior)</param>
        public PriorityQueue(int maxNodes)
        {
            if (maxNodes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxNodes), "New queue size cannot be smaller than 1");
            }

            this.Count = 0;
            this.nodes = new T[maxNodes + 1];
            this.numNodesEverEnqueued = 0;
        }

        /// <summary>
        ///     Returns the maximum number of items that can be enqueued at once in this queue.  Once you hit this number (ie. once
        ///     Count == MaxSize),
        ///     attempting to enqueue another item will cause undefined behavior.  O(1)
        /// </summary>
        public int MaxSize => this.nodes.Length - 1;

        /// <summary>
        ///     Returns the number of nodes in the queue.
        ///     O(1)
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Removes every node from the queue.
        ///     O(n) (So, don't do this often!)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(this.nodes, 1, this.Count);
            this.Count = 0;
        }

        /// <summary>
        ///     Returns (in O(1)!) whether the given node is in the queue.  O(1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return this.nodes[node.QueueIndex] == node;
        }

        /// <summary>
        ///     Enqueue a node to the priority queue.  Lower values are placed in front. Ties are broken by first-in-first-out.
        ///     If the queue is full, the result is undefined.
        ///     If the node is already enqueued, the result is undefined.
        ///     O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T node, int priority)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            
            if (this.Contains(node))
            {
                throw new ArgumentException(nameof(node), "Node is already enqueued: " + node);
            }
            
            if (this.Count == this.MaxSize)
            {
                this.Resize(this.MaxSize * 2 + 1);
            }

            node.Priority = priority;
            this.Count++;
            this.nodes[this.Count] = node;
            node.QueueIndex = this.Count;
            node.InsertionIndex = this.numNodesEverEnqueued++;
            this.CascadeUp(this.nodes[this.Count]);
        }

        /// <summary>
        ///     Removes the head of the queue (node with minimum priority; ties are broken by order of insertion), and returns it.
        ///     If queue is empty, result is undefined
        ///     O(log n)
        /// </summary>
        public T Dequeue()
        {
            if (this.Count <= 0)
            {
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
            }

            var returnMe = this.nodes[1];
            this.Remove(returnMe);
            return returnMe;
        }

        /// <summary>
        ///     Returns the head of the queue, without removing it (use Dequeue() for that).
        ///     If the queue is empty, behavior is undefined.
        ///     O(1)
        /// </summary>
        public T First
        {
            get
            {
                if (this.Count <= 0)
                {
                    throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
                }
                
                return this.nodes[1];
            }
        }

        /// <summary>
        ///     This method must be called on a node every time its priority changes while it is in the queue.
        ///     <b>Forgetting to call this method will result in a corrupted queue!</b>
        ///     Calling this method on a node not in the queue results in undefined behavior
        ///     O(log n)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePriority(T node, int priority)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (!this.Contains(node))
            {
                throw new ArgumentException(nameof(node), "Cannot call UpdatePriority() on a node which is not enqueued: " + node);
            }

            node.Priority = priority;
            this.OnNodeUpdated(node);
        }

        /// <summary>
        ///     Removes a node from the queue.  The node does not need to be the head of the queue.
        ///     If the node is not in the queue, the result is undefined.  If unsure, check Contains() first
        ///     O(log n)
        /// </summary>
        public void Remove(T node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (!this.Contains(node))
            {
                throw new ArgumentException(nameof(node), "Cannot call Remove() on a node which is not enqueued: " + node);
            }

            //If the node is already the last node, we can remove it immediately
            if (node.QueueIndex == this.Count)
            {
                this.nodes[this.Count] = null;
                this.Count--;
                return;
            }

            //Swap the node with the last node
            var formerLastNode = this.nodes[this.Count];
            this.Swap(node, formerLastNode);
            this.nodes[this.Count] = null;
            this.Count--;

            //Now bubble formerLastNode (which is no longer the last node) up or down as appropriate
            this.OnNodeUpdated(formerLastNode);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 1; i <= this.Count; i++)
                yield return this.nodes[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(T node1, T node2)
        {
            //Swap the nodes
            this.nodes[node1.QueueIndex] = node2;
            this.nodes[node2.QueueIndex] = node1;

            //Swap their indicies
            var temp = node1.QueueIndex;
            node1.QueueIndex = node2.QueueIndex;
            node2.QueueIndex = temp;
        }

        //Performance appears to be slightly better when this is NOT inlined o_O
        private void CascadeUp(T node)
        {
            //aka Heapify-up
            var parent = node.QueueIndex / 2;
            while (parent >= 1)
            {
                var parentNode = this.nodes[parent];
                if (HasHigherPriority(parentNode, node))
                    break;

                //Node has lower priority value, so move it up the heap
                this.Swap(
                    node,
                    parentNode); //For some reason, this is faster with Swap() rather than (less..?) individual operations, like in CascadeDown()

                parent = node.QueueIndex / 2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CascadeDown(T node)
        {
            //aka Heapify-down
            var finalQueueIndex = node.QueueIndex;
            while (true)
            {
                var newParent = node;
                var childLeftIndex = 2 * finalQueueIndex;

                //Check if the left-child is higher-priority than the current node
                if (childLeftIndex > this.Count)
                {
                    //This could be placed outside the loop, but then we'd have to check newParent != node twice
                    node.QueueIndex = finalQueueIndex;
                    this.nodes[finalQueueIndex] = node;
                    break;
                }

                var childLeft = this.nodes[childLeftIndex];
                if (HasHigherPriority(childLeft, newParent))
                {
                    newParent = childLeft;
                }

                //Check if the right-child is higher-priority than either the current node or the left child
                var childRightIndex = childLeftIndex + 1;
                if (childRightIndex <= this.Count)
                {
                    var childRight = this.nodes[childRightIndex];
                    if (HasHigherPriority(childRight, newParent))
                    {
                        newParent = childRight;
                    }
                }

                //If either of the children has higher (smaller) priority, swap and continue cascading
                if (newParent != node)
                {
                    //Move new parent to its new index.  node will be moved once, at the end
                    //Doing it this way is one less assignment operation than calling Swap()
                    this.nodes[finalQueueIndex] = newParent;

                    var temp = newParent.QueueIndex;
                    newParent.QueueIndex = finalQueueIndex;
                    finalQueueIndex = temp;
                }
                else
                {
                    //See note above
                    node.QueueIndex = finalQueueIndex;
                    this.nodes[finalQueueIndex] = node;
                    break;
                }
            }
        }

        /// <summary>
        ///     Returns true if 'higher' has higher priority than 'lower', false otherwise.
        ///     Note that calling HasHigherPriority(node, node) (ie. both arguments the same node) will return false
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasHigherPriority(T higher, T lower)
        {
            return higher.Priority < lower.Priority
                   || higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex;
        }

        /// <summary>
        ///     Resize the queue so it can accept more nodes.  All currently enqueued nodes are remain.
        ///     Attempting to decrease the queue size to a size too small to hold the existing nodes results in undefined behavior
        ///     O(n)
        /// </summary>
        public void Resize(int maxNodes)
        {
            if (maxNodes < 1)
            {
                throw new ArgumentException(nameof(maxNodes), "Queue size cannot be smaller than 1");
            }

            if (maxNodes < this.Count)
            {
                throw new ArgumentException(nameof(maxNodes), "Called Resize(" + maxNodes + "), but current queue contains " + this.Count + " nodes");
            }

            var newArray = new T[maxNodes + 1];
            var highestIndexToCopy = Math.Min(maxNodes, this.Count);
            for (var i = 1; i <= highestIndexToCopy; i++)
            {
                newArray[i] = this.nodes[i];
            }

            this.nodes = newArray;
        }

        private void OnNodeUpdated(T node)
        {
            //Bubble the updated node up or down as appropriate
            var parentIndex = node.QueueIndex / 2;
            var parentNode = this.nodes[parentIndex];

            if (parentIndex > 0 && HasHigherPriority(node, parentNode))
            {
                this.CascadeUp(node);
            }
            else
            {
                //Note that CascadeDown will be called if parentNode == node (that is, node is the root)
                this.CascadeDown(node);
            }
        }

        /// <summary>
        ///     <b>Should not be called in production code.</b>
        ///     Checks to make sure the queue is still in a valid state.  Used for testing/debugging the queue.
        /// </summary>
        public bool IsValidQueue()
        {
            for (var i = 1; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    var childLeftIndex = 2 * i;
                    if (childLeftIndex < this.nodes.Length && this.nodes[childLeftIndex] != null
                                                           && HasHigherPriority(
                                                               this.nodes[childLeftIndex],
                                                               this.nodes[i]))
                        return false;

                    var childRightIndex = childLeftIndex + 1;
                    if (childRightIndex < this.nodes.Length && this.nodes[childRightIndex] != null
                                                            && HasHigherPriority(
                                                                this.nodes[childRightIndex],
                                                                this.nodes[i]))
                        return false;
                }
            }

            return true;
        }
    }

    public class PriorityQueueNode
    {
        /// <summary>
        ///     The Priority to insert this node at.  Must be set BEFORE adding a node to the queue
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///     <b>Used by the priority queue - do not edit this value.</b>
        ///     Represents the order the node was inserted in
        /// </summary>
        public long InsertionIndex { get; set; }

        /// <summary>
        ///     <b>Used by the priority queue - do not edit this value.</b>
        ///     Represents the current position in the queue
        /// </summary>
        internal int QueueIndex { get; set; }
    }
}