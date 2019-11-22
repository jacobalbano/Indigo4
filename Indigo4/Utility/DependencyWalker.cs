using System;
using System.Collections.Generic;
using System.Linq;

namespace Indigo.Utility
{
    /// <summary>
    /// Collection that allows for traversal of a graph of dependent objects
    /// Can also be enumerated directly as a topographically-ordered list
    /// </summary>
    /// <typeparam name="T">Type of the object to store</typeparam>
    public class DependencyWalker<T> : IEnumerable<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">A collection of all items</param>
        /// <param name="determineDependencies"></param>
        public DependencyWalker(IEnumerable<T> items, Func<T, IEnumerable<T>> determineDependencies)
        {
            Graph = MakeGraph(items, determineDependencies);

            //  check for circular dependencies
            Walk(n => true);
        }

        /// <summary>
        /// Walk the graph and perform an action on each node.
        /// </summary>
        /// <param name="processNode">Node handler</param>
        public void Walk(Action<T> processNode)
        {
            Walk((n, unmetDeps) => {
                processNode(n);
                return true;
            });
        }

        /// <summary>
        /// Walk the graph and perform an action on each node.
        /// Return true to continue the traversal
        /// Return false to prevent other dependent nodes from being visited
        /// </summary>
        /// <param name="processNode">Node handler</param>
        public void Walk(Func<T, bool> processNode)
        {
            Walk((n, unmetDeps) => {
                bool result = false;

                if (!unmetDeps.Any())
                    result = processNode(n);

                return result;
            });
        }

        /// <summary>
        /// Walk the graph and perform an action on each node, given a list of dependencies that are unmet
        /// Return true to continue the traversal
        /// Return false to indicate that the current node should be considered an unmet dependency
        /// </summary>
        /// <param name="processNode">Node handler</param>
        public void Walk(Func<T, T[], bool> processNode)
        {
            if (processNode == null)
                throw new ArgumentNullException("processNode");

            var root = new DependencyNode();
            root.Dependencies.AddRange(Graph);

            var failedDeps = new HashSet<T>();

            foreach (var n in ResolveOrderInternal(root))
            {
                if (n == root)
                    continue;

                var unmetDependencies = failedDeps
                    .Intersect(n.Dependencies.Select(i => i.Item))
                    .ToArray();

                bool result = processNode(n.Item, unmetDependencies);
                if (!result)
                {
                    failedDeps.Add(n.Item);
                    continue;
                }
            }
        }

        #region IEnumerable implementation
        public IEnumerator<T> GetEnumerator()
        {
            var results = new List<T>();
            Walk(n => results.Add(n));
            return results.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        private IEnumerable<DependencyNode> ResolveOrderInternal(DependencyNode node, HashSet<DependencyNode> resolved = null, HashSet<DependencyNode> unresolved = null)
        {
            resolved = resolved ?? new HashSet<DependencyNode>();
            unresolved = unresolved ?? new HashSet<DependencyNode>();
            unresolved.Add(node);

            foreach (var child in node.Dependencies)
            {
                if (!resolved.Contains(child))
                {
                    if (unresolved.Contains(child))
                        throw new Exception(string.Format("Circular dependency: {0} <-> {1}", node.Item, child.Item));

                    foreach (var item in ResolveOrderInternal(child, resolved, unresolved))
                        yield return item;
                }
            }

            resolved.Add(node);
            unresolved.Remove(node);
            yield return node;
        }

        private List<DependencyNode> MakeGraph(IEnumerable<T> items, Func<T, IEnumerable<T>> determineDependencies)
        {
            var allItems = new HashSet<T>(items);
            var newItems = allItems.ToList();

            //  expand items to be considered by walking dependencies until no more are found
            while (newItems.Count != 0)
            {
                var expand = newItems
                    .SelectMany(determineDependencies)
                    .Where(i => !allItems.Contains(i))
                    .ToList();

                newItems = expand;
                expand.ForEach(i => allItems.Add(i));
            }

            var nodes = allItems
                .Select(i => new DependencyNode { Item = i })
                .ToList();

            var lookup = nodes.ToDictionary(n => n.Item);

            foreach (var i in allItems)
            {
                foreach (var dep in determineDependencies(i))
                    lookup[i].Dependencies.Add(lookup[dep]);
            }

            return nodes;
        }

        private readonly List<DependencyNode> Graph;

        private class DependencyNode
        {
            public T Item;
            public readonly List<DependencyNode> Dependencies = new List<DependencyNode>();
        }
    }
}