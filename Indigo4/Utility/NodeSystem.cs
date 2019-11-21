
using System;
using System.Collections.Generic;

namespace Indigo.Utility
{
	/// <summary>
	/// Small wrapper around a list of nodes, with a few utility functions.
	/// </summary>
	public class NodeSystem : IEnumerable<NodeSystem.Node>
	{
		public class Node
		{
			public bool IsHead;
			public Node Next, Prev;
			public int X, Y;
		}
		
		private List<Node> Nodes;
		
		/// <summary>
		/// The number of nodes currently held.
		/// </summary>
		public int Count
		{
			get { return Nodes.Count; }
		}
		
		public Node this[int index]
		{
			get { return Nodes[index]; }
		}
		
		public NodeSystem() : this(new List<Node>())
		{
		}
		
		public NodeSystem(List<Node> nodeList)
		{
			Synch(nodeList);
		}
		
		public void Synch(List<Node> nodeList)
		{
            Nodes = nodeList ?? throw new ArgumentNullException("nodeList");
		}
		
		public IEnumerable<Node> HeadNodes()
		{
			foreach (var node in this)
				if (node.IsHead) yield return node;
		}
		
		public IEnumerable<Node> EndNodes()
		{
			foreach (var node in this)
				if (node.Next == null || node.Prev == null) yield return node;
		}
		
		public void Remove(Node node)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerator<NodeSystem.Node> GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}
	}
}
