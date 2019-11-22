using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Indigo.Core.Collections
{
    public class BufferedCollection2<T> : IEnumerable<T>
    {
        public enum DeltaOperation { None, Add, Remove }

        public struct Delta
        {
            public DeltaOperation Operation { get; set; }
            public T Item { get; set; }
        }

        public void Sync()
        {
            foreach (var d in SyncYieldingDeltas())
                ;
        }

        public IEnumerable<Delta> SyncYieldingDeltas()
        {
            if (!dirty) yield break;
            if (Interlocked.Exchange(ref syncing, SYNCINC) != NOTSYNCING)
                throw new InvalidOperationException("SyncYieldingDeltas() was called while another sync was in progress");

            var newHead = head;
            for (int i = items.Count; i-- > head;)
            {
                newHead++;
                yield return new Delta { Operation = DeltaOperation.Add, Item = items[i] };
            }

            if (skip.Any())
            {
                var orderedIndexes = skip.OrderByDescending(x => x).ToList();
                skip.Clear();

                for (int i = orderedIndexes.Count; i-- > 0;)
                {
                    var index = orderedIndexes[i];
                    var item = items[index];
                    items.RemoveAt(index);
                    newHead--;
                    yield return new Delta { Operation = DeltaOperation.Remove, Item = item }; ;
                }
            }

            syncing = NOTSYNCING;
            dirty = false;
            head = newHead;
        }

        public void Add(T item)
        {
            dirty = true;
            items.Add(item);
        }

        public bool Remove(T item)
        {
            var index = items.IndexOf(item);
            if (index >= 0)
            {
                dirty = true;
                if (index > head)
                    items.RemoveAt(index);
                else
                    skip.Add(index);
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = head;
            while (i -->  0)
            {
                if (!skip.Contains(i))
                    yield return items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly HashSet<int> skip = new HashSet<int>();
        private readonly List<T> items = new List<T>();
        private bool dirty = false;
        private volatile int syncing = NOTSYNCING;
        private int head = 0;

        const int NOTSYNCING = 0, SYNCINC = 1;
    }
}
