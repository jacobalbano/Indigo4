using System;
using System.Collections.Generic;
using System.Linq;

namespace Indigo.Utility
{
    public class TypeHash<TBaseClass>
    {
        private Dictionary<Type, List<TBaseClass>> hash;

        public TypeHash()
        {
            Clear();
        }

        public void Clear()
        {
            hash = new Dictionary<Type, List<TBaseClass>>();
        }
        
        public IEnumerable<T> Get<T>(bool includeSubclasses = false) where T : TBaseClass
        {
            if (includeSubclasses)
            {
                foreach (var pair in hash.Where(p => typeof(T).IsAssignableFrom(p.Key)))
                {
                    foreach (var item in pair.Value)
                        yield return (T)item;
                }
            }
            else
            {
                if (!hash.TryGetValue(typeof(T), out var list))
                    yield break;

                foreach (T item in list)
                    yield return item;
            }
        }

        public void Add(TBaseClass item)
        {
            var t = item.GetType();
            if (!hash.TryGetValue(t, out var list))
                hash[t] = list = new List<TBaseClass>();

            list.Add(item);
        }

        public void Remove(TBaseClass item)
        {
            var t = item.GetType();
            if (!hash.TryGetValue(t, out var list))
                throw new Exception("Item is not contained within this hash");

            list.Remove(item);
        }
    }
}
