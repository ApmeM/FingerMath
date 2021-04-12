namespace FingerMath.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using FingerMath;

    /// <summary>
    ///     sourced from: https://github.com/tejacques/Deque
    ///     A genetic Deque class. It can be thought of as a double-ended queue, hence Deque. This allows for
    ///     an O(1) AddFront, AddBack, RemoveFront, RemoveBack. The Deque also has O(1) indexed lookup, as it is backed
    ///     by a circular array.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of objects to store in the deque.
    /// </typeparam>
    public class Deque<T> : IList<T>
    {
        /// <summary>
        ///     The default capacity of the deque.
        /// </summary>
        private const int DefaultCapacity = 16;

        /// <summary>
        ///     The circular array holding the items.
        /// </summary>
        private T[] buffer;

        private int capacityClosestPowerOfTwoMinusOne;

        /// <summary>
        ///     The first element offset from the beginning of the data array.
        /// </summary>
        private int startOffset;

        /// <summary>
        ///     Creates a new instance of the Deque class with
        ///     the default capacity.
        /// </summary>
        public Deque()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        ///     Creates a new instance of the Deque class with
        ///     the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the Deque.</param>
        public Deque(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "capacity is less than 0.");
            }

            this.Capacity = capacity;
        }

        /// <summary>
        ///     Create a new instance of the Deque class with the elements
        ///     from the specified collection.
        /// </summary>
        /// <param name="collection">The co</param>
        public Deque(ICollection<T> collection)
            : this(collection.Count)
        {
            this.InsertRange(0, collection);
        }

        /// <summary>
        ///     Gets or sets the total number of elements
        ///     the internal array can hold without resizing.
        /// </summary>
        public int Capacity
        {
            get => this.buffer.Length;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Capacity is less than 0.");
                }

                if (value < this.Count)
                {
                    throw new InvalidOperationException("Capacity cannot be set to a value less than Count");
                }

                if (null != this.buffer && value == this.buffer.Length)
                {
                    return;
                }

                // Create a new array and copy the old values.
                var powOfTwo = Mathf.ClosestPowerOfTwoGreaterThan(value);

                value = powOfTwo;

                var newBuffer = new T[value];
                this.CopyTo(newBuffer, 0);

                // Set up to use the new buffer.
                this.buffer = newBuffer;
                this.startOffset = 0;
                this.capacityClosestPowerOfTwoMinusOne = powOfTwo - 1;
            }
        }

        /// <summary>
        ///     Gets whether or not the Deque is filled to capacity.
        /// </summary>
        public bool IsFull => this.Count == this.Capacity;

        /// <summary>
        ///     Gets whether or not the Deque is empty.
        /// </summary>
        public bool IsEmpty => 0 == this.Count;

        private void EnsureCapacityFor(int numElements)
        {
            if (this.Count + numElements > this.Capacity)
            {
                this.Capacity = this.Count + numElements;
            }
        }

        private int ToBufferIndex(int index)
        {
            return (index + this.startOffset) & this.capacityClosestPowerOfTwoMinusOne;
        }

        private void CheckIndexOutOfRange(int index)
        {
            if (index >= this.Count)
            {
                throw new IndexOutOfRangeException("The supplied index is greater than the Count");
            }
        }

        private static void CheckArgumentsOutOfRange(int length, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Invalid offset " + offset);
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Invalid count " + count);
            }

            if (length - offset < count)
            {
                throw new ArgumentException(
                    $"Invalid offset ({offset}) or count + ({count}) " + $"for source length {length}");
            }
        }

        private int ShiftStartOffset(int value)
        {
            this.startOffset = this.ToBufferIndex(value);

            return this.startOffset;
        }

        private int PreShiftStartOffset(int value)
        {
            var offset = this.startOffset;
            this.ShiftStartOffset(value);
            return offset;
        }

        private int PostShiftStartOffset(int value)
        {
            return this.ShiftStartOffset(value);
        }

        /// <summary>
        ///     Adds the provided item to the front of the Deque.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddFront(T item)
        {
            this.EnsureCapacityFor(1);
            this.buffer[this.PostShiftStartOffset(-1)] = item;
            this.IncrementCount(1);
        }

        /// <summary>
        ///     Adds the provided item to the back of the Deque.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddBack(T item)
        {
            this.EnsureCapacityFor(1);
            this.buffer[this.ToBufferIndex(this.Count)] = item;
            this.IncrementCount(1);
        }

        /// <summary>
        ///     Removes an item from the front of the Deque and returns it.
        /// </summary>
        /// <returns>The item at the front of the Deque.</returns>
        public T RemoveFront()
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("The Deque is empty");
            }

            var result = this.buffer[this.startOffset];
            this.buffer[this.PreShiftStartOffset(1)] = default(T);
            this.DecrementCount(1);
            return result;
        }

        /// <summary>
        ///     Removes an item from the back of the Deque and returns it.
        /// </summary>
        /// <returns>The item in the back of the Deque.</returns>
        public T RemoveBack()
        {
            if (this.IsEmpty)
                throw new InvalidOperationException("The Deque is empty");

            this.DecrementCount(1);
            var endIndex = this.ToBufferIndex(this.Count);
            var result = this.buffer[endIndex];
            this.buffer[endIndex] = default(T);

            return result;
        }

        /// <summary>
        ///     Adds a collection of items to the Deque.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void AddRange(ICollection<T> collection)
        {
            this.AddBackRange(collection);
        }

        /// <summary>
        ///     Adds a collection of items to the front of the Deque.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void AddFrontRange(ICollection<T> collection)
        {
            this.AddFrontRange(collection, 0, collection.Count);
        }

        /// <summary>
        ///     Adds count items from a collection of items
        ///     from a specified index to the Deque.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        /// <param name="fromIndex">
        ///     The index in the collection to begin adding from.
        /// </param>
        /// <param name="count">
        ///     The number of items in the collection to add.
        /// </param>
        public void AddFrontRange(IEnumerable<T> collection, int fromIndex, int count)
        {
            this.InsertRange(0, collection, fromIndex, count);
        }

        /// <summary>
        ///     Adds a collection of items to the back of the Deque.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void AddBackRange(ICollection<T> collection)
        {
            this.AddBackRange(collection, 0, collection.Count);
        }

        /// <summary>
        ///     Adds count items from a collection of items
        ///     from a specified index to the back of the Deque.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        /// <param name="fromIndex">
        ///     The index in the collection to begin adding from.
        /// </param>
        /// <param name="count">
        ///     The number of items in the collection to add.
        /// </param>
        public void AddBackRange(IEnumerable<T> collection, int fromIndex, int count)
        {
            this.InsertRange(this.Count, collection, fromIndex, count);
        }

        /// <summary>
        ///     Inserts a collection of items into the Deque
        ///     at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The index in the Deque to insert the collection.
        /// </param>
        /// <param name="collection">The collection to add.</param>
        public void InsertRange(int index, ICollection<T> collection)
        {
            var count = collection.Count;
            this.InsertRange(index, collection, 0, count);
        }

        /// <summary>
        ///     Inserts count items from a collection of items from a specified
        ///     index into the Deque at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The index in the Deque to insert the collection.
        /// </param>
        /// <param name="collection">The collection to add.</param>
        /// <param name="fromIndex">
        ///     The index in the collection to begin adding from.
        /// </param>
        /// <param name="count">
        ///     The number of items in the colleciton to add.
        /// </param>
        public void InsertRange(int index, IEnumerable<T> collection, int fromIndex, int count)
        {
            this.CheckIndexOutOfRange(index - 1);

            if (0 == count)
            {
                return;
            }

            // Make room
            this.EnsureCapacityFor(count);

            if (index < this.Count / 2)
            {
                // Inserting into the first half of the list

                if (index > 0)
                {
                    // Move items down:
                    //  [0, index) -> 
                    //  [Capacity - count, Capacity - count + index)
                    var copyCount = index;
                    var shiftIndex = this.Capacity - count;
                    for (var j = 0; j < copyCount; j++)
                    {
                        this.buffer[this.ToBufferIndex(shiftIndex + j)] = this.buffer[this.ToBufferIndex(j)];
                    }
                }

                // shift the starting offset
                this.ShiftStartOffset(-count);
            }
            else
            {
                // Inserting into the second half of the list

                if (index < this.Count)
                {
                    // Move items up:
                    // [index, Count) -> [index + count, count + Count)
                    var copyCount = this.Count - index;
                    var shiftIndex = index + count;
                    for (var j = 0; j < copyCount; j++)
                    {
                        this.buffer[this.ToBufferIndex(shiftIndex + j)] = this.buffer[this.ToBufferIndex(index + j)];
                    }
                }
            }

            // Copy new items into place
            var i = index;
            foreach (var item in collection)
            {
                this.buffer[this.ToBufferIndex(i)] = item;
                ++i;
            }

            // Adjust valid count
            this.IncrementCount(count);
        }

        /// <summary>
        ///     Removes a range of elements from the view.
        /// </summary>
        /// <param name="index">
        ///     The index into the view at which the range begins.
        /// </param>
        /// <param name="count">
        ///     The number of elements in the range. This must be greater
        ///     than 0 and less than or equal to <see cref="Count" />.
        /// </param>
        public void RemoveRange(int index, int count)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("The Deque is empty");
            }

            if (index > this.Count - count)
            {
                throw new IndexOutOfRangeException("The supplied index is greater than the Count");
            }

            // Clear out the underlying array
            this.ClearBuffer(index, count);

            if (index == 0)
            {
                // Removing from the beginning: shift the start offset
                this.ShiftStartOffset(count);
                this.Count -= count;
                return;
            }

            if (index == this.Count - count)
            {
                // Removing from the ending: trim the existing view
                this.Count -= count;
                return;
            }

            if (index + count / 2 < this.Count / 2)
            {
                // Removing from first half of list

                // Move items up:
                //  [0, index) -> [count, count + index)
                var copyCount = index;
                var writeIndex = count;
                for (var j = 0; j < copyCount; j++)
                {
                    this.buffer[this.ToBufferIndex(writeIndex + j)] = this.buffer[this.ToBufferIndex(j)];
                }

                // Rotate to new view
                this.ShiftStartOffset(count);
            }
            else
            {
                // Removing from second half of list

                // Move items down:
                // [index + collectionCount, count) ->
                // [index, count - collectionCount)
                var copyCount = this.Count - count - index;
                var readIndex = index + count;
                for (var j = 0; j < copyCount; ++j)
                {
                    this.buffer[this.ToBufferIndex(index + j)] = this.buffer[this.ToBufferIndex(readIndex + j)];
                }
            }

            // Adjust valid count
            this.DecrementCount(count);
        }

        /// <summary>
        ///     Gets the value at the specified index of the Deque
        /// </summary>
        /// <param name="index">The index of the Deque.</param>
        /// <returns></returns>
        public T Get(int index)
        {
            this.CheckIndexOutOfRange(index);
            return this.buffer[this.ToBufferIndex(index)];
        }

        /// <summary>
        ///     Sets the value at the specified index of the
        ///     Deque to the given item.
        /// </summary>
        /// <param name="index">The index of the deque to set the item.</param>
        /// <param name="item">The item to set at the specified index.</param>
        public void Set(int index, T item)
        {
            this.CheckIndexOutOfRange(index);
            this.buffer[this.ToBufferIndex(index)] = item;
        }

        #region IEnumerable

        /// <summary>
        ///     Returns an enumerator that iterates through the Deque.
        /// </summary>
        /// <returns>
        ///     An iterator that can be used to iterate through the Deque.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            // The below is done for performance reasons.
            // Rather than doing bounds checking and modulo arithmetic
            // that would go along with calls to Get(index), we can skip
            // all of that by referencing the underlying array.

            if (this.startOffset + this.Count > this.Capacity)
            {
                for (var i = this.startOffset; i < this.Capacity; i++)
                {
                    yield return this.buffer[i];
                }

                var endIndex = this.ToBufferIndex(this.Count);
                for (var i = 0; i < endIndex; i++)
                {
                    yield return this.buffer[i];
                }
            }
            else
            {
                var endIndex = this.startOffset + this.Count;
                for (var i = this.startOffset; i < endIndex; i++)
                {
                    yield return this.buffer[i];
                }
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the Deque.
        /// </summary>
        /// <returns>
        ///     An iterator that can be used to iterate through the Deque.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region ICollection

        /// <summary>
        ///     Gets a value indicating whether the Deque is read-only.
        /// </summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        ///     Gets the number of elements contained in the Deque.
        /// </summary>
        public int Count { get; set; }

        private void IncrementCount(int value)
        {
            this.Count = this.Count + value;
        }

        private void DecrementCount(int value)
        {
            this.Count = Math.Max(this.Count - value, 0);
        }

        /// <summary>
        ///     Adds an item to the Deque.
        /// </summary>
        /// <param name="item">The object to add to the Deque.</param>
        public void Add(T item)
        {
            this.AddBack(item);
        }

        private void ClearBuffer(int logicalIndex, int length)
        {
            var offset = this.ToBufferIndex(logicalIndex);
            if (offset + length > this.Capacity)
            {
                var len = this.Capacity - offset;
                Array.Clear(this.buffer, offset, len);

                len = this.ToBufferIndex(logicalIndex + length);
                Array.Clear(this.buffer, 0, len);
            }
            else
            {
                Array.Clear(this.buffer, offset, length);
            }
        }

        /// <summary>
        ///     Removes all items from the Deque.
        /// </summary>
        public void Clear()
        {
            if (this.Count > 0)
            {
                this.ClearBuffer(0, this.Count);
            }

            this.Count = 0;
            this.startOffset = 0;
        }

        /// <summary>
        ///     Determines whether the Deque contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the Deque.</param>
        /// <returns>
        ///     true if item is found in the Deque; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }

        /// <summary>
        ///     Copies the elements of the Deque to a System.Array,
        ///     starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional System.Array that is the destination of
        ///     the elements copied from the Deque. The System.Array must
        ///     have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     array is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     arrayIndex is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     The number of elements in the source Deque is greater than
        ///     the available space from arrayIndex to the end of the
        ///     destination array.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (null == array)
            {
                throw new ArgumentNullException(nameof(array), "Array is null");
            }

            // Nothing to copy
            if (null == this.buffer)
            {
                return;
            }

            CheckArgumentsOutOfRange(array.Length, arrayIndex, this.Count);

            if (0 != this.startOffset && this.startOffset + this.Count >= this.Capacity)
            {
                var lengthFromStart = this.Capacity - this.startOffset;
                var lengthFromEnd = this.Count - lengthFromStart;

                Array.Copy(this.buffer, this.startOffset, array, 0, lengthFromStart);

                Array.Copy(this.buffer, 0, array, lengthFromStart, lengthFromEnd);
            }
            else
            {
                Array.Copy(this.buffer, this.startOffset, array, 0, this.Count);
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the Deque.
        /// </summary>
        /// <param name="item">The object to remove from the Deque.</param>
        /// <returns>
        ///     true if item was successfully removed from the Deque;
        ///     otherwise, false. This method also returns false if item
        ///     is not found in the original
        /// </returns>
        public bool Remove(T item)
        {
            var result = true;
            var index = this.IndexOf(item);

            if (-1 == index)
            {
                result = false;
            }
            else
            {
                this.RemoveAt(index);
            }

            return result;
        }

        #endregion

        #region List<T>

        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index of the element to get or set.
        /// </param>
        /// <returns>The element at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in this deque
        /// </exception>
        public T this[int index]
        {
            get => this.Get(index);

            set => this.Set(index, value);
        }

        /// <summary>
        ///     Inserts an item to the Deque at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">The object to insert into the Deque.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the Deque.
        /// </exception>
        public void Insert(int index, T item)
        {
            this.EnsureCapacityFor(1);

            if (index == 0)
            {
                this.AddFront(item);
                return;
            }

            if (index == this.Count)
            {
                this.AddBack(item);
                return;
            }

            this.InsertRange(index, new[] { item });
        }

        /// <summary>
        ///     Determines the index of a specific item in the deque.
        /// </summary>
        /// <param name="item">The object to locate in the deque.</param>
        /// <returns>
        ///     The index of the item if found in the deque; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            var index = 0;
            foreach (var myItem in this)
            {
                if (myItem.Equals(item))
                {
                    break;
                }

                ++index;
            }

            if (index == this.Count)
            {
                index = -1;
            }

            return index;
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index of the item to remove.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the Deque.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                this.RemoveFront();
                return;
            }

            if (index == this.Count - 1)
            {
                this.RemoveBack();
                return;
            }

            this.RemoveRange(index, 1);
        }

        #endregion
    }
}