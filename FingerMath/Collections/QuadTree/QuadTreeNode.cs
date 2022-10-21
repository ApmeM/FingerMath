namespace FingerMath.Collections.QuadTree
{
    using FingerMath.Primitives;
    using System.Collections.Generic;

    /// <summary>
    ///     A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeNode<T>
        where T : IQuadTreeStorable
    {
        // How many objects can exist in a QuadTree before it sub divides itself
        private const int MaxObjectsPerNode = 2;

        // The objects in this QuadTree
        private List<QuadTreeObject<T>> objects;

        /// <summary>
        ///     Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="quadRect">The area this QuadTree object will encompass.</param>
        public QuadTreeNode(RectangleF quadRect)
        {
            this.QuadRect = quadRect;
        }

        /// <summary>
        ///     Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area rectangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        public QuadTreeNode(int x, int y, int width, int height)
        {
            this.QuadRect = new RectangleF(x, y, width, height);
        }

        private QuadTreeNode(QuadTreeNode<T> parent, RectangleF rect)
            : this(rect)
        {
            this.Parent = parent;
        }

        /// <summary>
        ///     The area this QuadTree represents.
        /// </summary>
        public RectangleF QuadRect { get; }

        /// <summary>
        ///     The top left child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> TopLeftChild { get; private set; }

        /// <summary>
        ///     The top right child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> TopRightChild { get; private set; }

        /// <summary>
        ///     The bottom left child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> BottomLeftChild { get; private set; }

        /// <summary>
        ///     The bottom right child for this QuadTree
        /// </summary>
        public QuadTreeNode<T> BottomRightChild { get; private set; }

        /// <summary>
        ///     This QuadTree's parent
        /// </summary>
        public QuadTreeNode<T> Parent { get; }

        /// <summary>
        ///     How many total objects are contained within this QuadTree (ie, includes children)
        /// </summary>
        public int Count => this.ObjectCount();

        /// <summary>
        ///     Returns true if this is a empty leaf node
        /// </summary>
        public bool IsEmptyLeaf => this.Count == 0 && this.TopLeftChild == null;

        /// <summary>
        ///     Add an item to the object list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(QuadTreeObject<T> item)
        {
            if (this.objects == null)
                this.objects = new List<QuadTreeObject<T>>();

            item.Owner = this;
            this.objects.Add(item);
        }

        /// <summary>
        ///     Remove an item from the object list.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        private void Remove(QuadTreeObject<T> item)
        {
            if (this.objects != null)
            {
                var removeIndex = this.objects.IndexOf(item);
                if (removeIndex >= 0)
                {
                    this.objects[removeIndex] = this.objects[this.objects.Count - 1];
                    this.objects.RemoveAt(this.objects.Count - 1);
                }
            }
        }

        /// <summary>
        ///     Get the total for all objects in this QuadTree, including children.
        /// </summary>
        /// <returns>The number of objects contained within this QuadTree and its children.</returns>
        private int ObjectCount()
        {
            var count = 0;

            // Add the objects at this level
            if (this.objects != null)
            {
                count += this.objects.Count;
            }

            // Add the objects that are contained in the children
            if (this.TopLeftChild != null)
            {
                count += this.TopLeftChild.ObjectCount();
                count += this.TopRightChild.ObjectCount();
                count += this.BottomLeftChild.ObjectCount();
                count += this.BottomRightChild.ObjectCount();
            }

            return count;
        }

        /// <summary>
        ///     Subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        private void Subdivide()
        {
            // We've reached capacity, subdivide...
            var sizeX = (int)this.QuadRect.Width / 2;
            var sizeY = (int)this.QuadRect.Height / 2;
            var midX = (int)this.QuadRect.X + sizeX;
            var midY = (int)this.QuadRect.Y + sizeY;

            this.TopLeftChild = new QuadTreeNode<T>(
                this,
                new RectangleF(this.QuadRect.Left, this.QuadRect.Top, sizeX, sizeY));
            this.TopRightChild = new QuadTreeNode<T>(this, new RectangleF(midX, this.QuadRect.Top, sizeX, sizeY));
            this.BottomLeftChild = new QuadTreeNode<T>(this, new RectangleF(this.QuadRect.Left, midY, sizeX, sizeY));
            this.BottomRightChild = new QuadTreeNode<T>(this, new RectangleF(midX, midY, sizeX, sizeY));

            // If they're completely contained by the quad, bump objects down
            for (var i = 0; i < this.objects.Count; i++)
            {
                var destTree = this.GetDestinationTree(this.objects[i]);

                if (destTree != this)
                {
                    // Insert to the appropriate tree, remove the object, and back up one in the loop
                    destTree.Insert(this.objects[i]);
                    this.Remove(this.objects[i]);
                    i--;
                }
            }
        }

        /// <summary>
        ///     Get the child Quad that would contain an object.
        /// </summary>
        /// <param name="item">The object to get a child for.</param>
        /// <returns></returns>
        private QuadTreeNode<T> GetDestinationTree(QuadTreeObject<T> item)
        {
            // If a child can't contain an object, it will live in this Quad
            var destTree = this;

            if (this.TopLeftChild.QuadRect.Contains(item.Data.Bounds))
                destTree = this.TopLeftChild;
            else if (this.TopRightChild.QuadRect.Contains(item.Data.Bounds))
                destTree = this.TopRightChild;
            else if (this.BottomLeftChild.QuadRect.Contains(item.Data.Bounds))
                destTree = this.BottomLeftChild;
            else if (this.BottomRightChild.QuadRect.Contains(item.Data.Bounds))
                destTree = this.BottomRightChild;

            return destTree;
        }

        private void Relocate(QuadTreeObject<T> item)
        {
            // Are we still inside our parent?
            if (this.QuadRect.Contains(item.Data.Bounds))
            {
                // Good, have we moved inside any of our children?
                if (this.TopLeftChild != null)
                {
                    var dest = this.GetDestinationTree(item);
                    if (item.Owner != dest)
                    {
                        // Delete the item from this quad and add it to our child
                        // Note: Do NOT clean during this call, it can potentially delete our destination quad
                        var formerOwner = item.Owner;
                        this.Delete(item, false);
                        dest.Insert(item);

                        // Clean up ourselves
                        formerOwner.CleanUpwards();
                    }
                }
            }
            else
            {
                // We don't fit here anymore, move up, if we can
                this.Parent?.Relocate(item);
            }
        }

        private void CleanUpwards()
        {
            if (this.TopLeftChild != null)
            {
                // If all the children are empty leaves, delete all the children
                if (this.TopLeftChild.IsEmptyLeaf && this.TopRightChild.IsEmptyLeaf && this.BottomLeftChild.IsEmptyLeaf
                    && this.BottomRightChild.IsEmptyLeaf)
                {
                    this.TopLeftChild = null;
                    this.TopRightChild = null;
                    this.BottomLeftChild = null;
                    this.BottomRightChild = null;

                    if (this.Parent != null && this.Count == 0)
                        this.Parent.CleanUpwards();
                }
            }
            else
            {
                // I could be one of 4 empty leaves, tell my parent to clean up
                if (this.Parent != null && this.Count == 0)
                    this.Parent.CleanUpwards();
            }
        }

        /// <summary>
        ///     Clears the QuadTree of all objects, including any objects living in its children.
        /// </summary>
        internal void Clear()
        {
            // Clear out the children, if we have any
            if (this.TopLeftChild != null)
            {
                this.TopLeftChild.Clear();
                this.TopRightChild.Clear();
                this.BottomLeftChild.Clear();
                this.BottomRightChild.Clear();
            }

            // Clear any objects at this level
            if (this.objects != null)
            {
                this.objects.Clear();
                this.objects = null;
            }

            // Set the children to null
            this.TopLeftChild = null;
            this.TopRightChild = null;
            this.BottomLeftChild = null;
            this.BottomRightChild = null;
        }

        /// <summary>
        ///     Deletes an item from this QuadTree. If the object is removed causes this Quad to have no objects in its children,
        ///     it's children will be removed as well.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="clean">Whether or not to clean the tree</param>
        internal void Delete(QuadTreeObject<T> item, bool clean)
        {
            if (item.Owner != null)
            {
                if (item.Owner == this)
                {
                    this.Remove(item);
                    if (clean)
                        this.CleanUpwards();
                }
                else
                {
                    item.Owner.Delete(item, clean);
                }
            }
        }

        /// <summary>
        ///     Insert an item into this QuadTree object.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        internal void Insert(QuadTreeObject<T> item)
        {
            // If this quad doesn't contain the items rectangle, do nothing, unless we are the root
            if (!this.QuadRect.Contains(item.Data.Bounds))
            {
                if (this.Parent == null)
                {
                    // This object is outside of the QuadTree bounds, we should add it at the root level
                    this.Add(item);
                }
                else
                {
                    return;
                }
            }

            if (this.objects == null || this.TopLeftChild == null && this.objects.Count + 1 <= MaxObjectsPerNode)
            {
                // If there's room to add the object, just add it
                this.Add(item);
            }
            else
            {
                // No quads, create them and bump objects down where appropriate
                if (this.TopLeftChild == null)
                    this.Subdivide();

                // Find out which tree this object should go in and add it there
                var destTree = this.GetDestinationTree(item);
                if (destTree == this)
                    this.Add(item);
                else
                    destTree.Insert(item);
            }
        }

        /// <summary>
        ///     Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        internal List<T> GetObjects(RectangleF searchRect)
        {
            var results = new List<T>();
            this.GetObjects(searchRect, ref results);
            return results;
        }

        /// <summary>
        ///     Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        internal void GetObjects(RectangleF searchRect, ref List<T> results)
        {
            // We can't do anything if the results list doesn't exist
            if (results != null)
            {
                if (searchRect.Contains(this.QuadRect))
                {
                    // If the search area completely contains this quad, just get every object this quad and all it's children have
                    this.GetAllObjects(ref results);
                }
                else if (searchRect.IsIntersects(this.QuadRect))
                {
                    // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                    if (this.objects != null)
                    {
                        for (var i = 0; i < this.objects.Count; i++)
                        {
                            if (searchRect.IsIntersects(this.objects[i].Data.Bounds))
                                results.Add(this.objects[i].Data);
                        }
                    }

                    // Get the objects for the search rectangle from the children
                    if (this.TopLeftChild != null)
                    {
                        this.TopLeftChild.GetObjects(searchRect, ref results);
                        this.TopRightChild.GetObjects(searchRect, ref results);
                        this.BottomLeftChild.GetObjects(searchRect, ref results);
                        this.BottomRightChild.GetObjects(searchRect, ref results);
                    }
                }
            }
        }

        /// <summary>
        ///     Get all objects in this Quad, and it's children.
        /// </summary>
        /// <param name="results">A reference to a list in which to store the objects.</param>
        internal void GetAllObjects(ref List<T> results)
        {
            // If this Quad has objects, add them
            if (this.objects != null)
            {
                for (var i = 0; i < this.objects.Count; i++)
                    results.Add(this.objects[i].Data);
            }

            // If we have children, get their objects too
            if (this.TopLeftChild != null)
            {
                this.TopLeftChild.GetAllObjects(ref results);
                this.TopRightChild.GetAllObjects(ref results);
                this.BottomLeftChild.GetAllObjects(ref results);
                this.BottomRightChild.GetAllObjects(ref results);
            }
        }

        /// <summary>
        ///     Moves the QuadTree object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        internal void Move(QuadTreeObject<T> item)
        {
            if (item.Owner != null)
                item.Owner.Relocate(item);
            else
                this.Relocate(item);
        }
    }
}