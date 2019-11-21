using System;
using System.Collections;
using System.Collections.Generic;

namespace Indigo.Utility
{
    /// <summary>Represents a collection of objects which each contain a collection of like objects, forming a simple tree.</summary>
    /// <typeparam name="TNode">Type of the items contained in the collection</typeparam>
    public class HierarchicalCollection<TNode> : HierarchicalCollection<TNode, HierarchicalCollection<TNode>>
        where TNode : class, HierarchicalCollection<TNode>.INode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="self">The node this collection is being assigned to. Items in this collection will have this node as their Parent.</param>
        /// <param name="setParent">
        /// A method that will set the Parent reference of children that are being added or removed.
        /// In the majority of cases, the following logic is sufficient: `(c, p) => c.Parent = p`
        /// This allows the Parent property to have a private setter in order to prevent meddling from outside.
        /// </param>
        public HierarchicalCollection(TNode self, ParentSetter setParent) : base(self, setParent)
        {
        }
    }

    /// <summary>Represents a collection of objects which each contain a collection of like objects, forming a simple tree.</summary>
    /// <typeparam name="TNode">Type of the items contained in the collection</typeparam>
    /// <typeparam name="TCollection">Specific type of the container each object will store its children in</typeparam>
    public class HierarchicalCollection<TNode, TCollection> : IEnumerable<TNode>
        where TNode : class, HierarchicalCollection<TNode, TCollection>.INode
        where TCollection : HierarchicalCollection<TNode, TCollection>
    {
        /// <summary>
        /// A delegate used to define the mechanism for setting the parent of a newly-added child node.
        /// Places responsibility on the implementing INode class.
        /// In the majority of cases, the following logic is sufficient: `(c, p) => c.Parent = p`
        /// </summary>
        /// <param name="child">The newly added or removed child node.</param>
        /// <param name="parent">The node the child is being added to or removed from</param>
        public delegate void ParentSetter(TNode child, TNode parent);

        /// <summary>The 'this' node that acts as Parent to the nodes contained in this collection.</summary>
        protected readonly TNode Self;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="self">The node this collection is being assigned to. Items in this collection will have this node as their Parent.</param>
        /// <param name="setParent">
        /// A method that will set the Parent reference of children that are being added or removed.
        /// In the majority of cases, the following logic is sufficient: `(c, p) => c.Parent = p`
        /// This allows the Parent property to have a private setter in order to prevent meddling from outside.
        /// </param>
        protected HierarchicalCollection(TNode self, ParentSetter setParent)
        {
            Self = self ?? throw new ArgumentNullException(nameof(self));
            SetParent = setParent ?? throw new ArgumentNullException(nameof(setParent));
            list = new List<TNode>();
        }

        /// <summary>
        /// Adds a new child to the collection. Child must be non-null and not be already assigned to a parent.
        /// </summary>
        /// <exception cref="ArgumentNullException">Child must not be null</exception>
        /// <exception cref="Exception">Child must not already be assigned to </exception>
        /// <param name="node"></param>
        public virtual void Add(TNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Parent != null)
                throw new Exception("Node already has a parent!");
            
            SetParent(node, Self);
            list.Add(node);
        }

        /// <summary>
        /// Removes a child from the collection. Child must be non-null and be assigned to this object
        /// </summary>
        /// <exception cref="ArgumentNullException">Child must not be null</exception>
        /// <exception cref="Exception">Child must be assigned to this object</exception>
        /// <param name="node"></param>
        public virtual void Remove(TNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Parent != Self)
                throw new Exception("Node is not a child of this node!");
            
            SetParent(node, null);
            list.Remove(node);
        }
        
        public IEnumerator<TNode> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Interface that stored items must implement</summary>
        public interface INode
        {
            /// <summary>The node that contains this object as a child (null if no assignment)</summary>
            TNode Parent { get; }

            /// <summary>Collection of nodes assigned as children to this object</summary>
            TCollection Children { get; }
        }

        private List<TNode> list;
        private readonly ParentSetter SetParent;
    }
}
