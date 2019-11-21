using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Utility
{
    public class PropertyHash<TKey, TObject> where TObject : class, INotifyPropertyChanged
    {
        private Dictionary<TObject, TKey> keysByObject;
        private Dictionary<TKey, List<TObject>> objectsByKey;
        private Func<TObject, TKey> GetKey;
        private List<TObject> nullKeyObjects;

        public int Count => nullKeyObjects.Count + keysByObject.Count;

        public IEnumerable<TKey> Keys => objectsByKey
            .Where(kv => kv.Value.Count > 0)
            .Select(kv => kv.Key);

        public PropertyHash(Func<TObject, TKey> getKey)
        {
            GetKey = getKey;
            InitializeCollections();
        }

        public IEnumerable<TObject> this[TKey key] => Get(key);

        public IEnumerable<TObject> Get(TKey key)
        {
            if (key == null)
            {
                foreach (var item in nullKeyObjects)
                    yield return item;
            }
            else
            {
                if (!objectsByKey.TryGetValue(key, out var result))
                    yield break;

                foreach (var item in result)
                    yield return item;
            }
        }

        public void Add(TObject o)
        {
            o.PropertyChanged += Object_PropertyChanged;

            var key = GetKey(o);
            if (key == null)
            {
                nullKeyObjects.Add(o);
            }
            else
            {
                if (!objectsByKey.TryGetValue(key, out var list))
                    objectsByKey[key] = list = new List<TObject>();


                keysByObject[o] = key;
                list.Add(o);
            }
        }

        public void AddRange(params TObject[] objects)
        {
            AddRange(objects.AsEnumerable());
        }

        public void AddRange(IEnumerable<TObject> objects)
        {
            foreach (var o in objects)
                Add(o);
        }

        public void Remove(TObject o)
        {
            o.PropertyChanged -= Object_PropertyChanged;

            var key = GetKey(o);
            if (key == null)
            {
                nullKeyObjects.Remove(o);
            }
            else
            {
                keysByObject.Remove(o);
                objectsByKey[key].Remove(o);
            }
        }

        public void Clear()
        {
            foreach (var o in keysByObject.Keys)
                o.PropertyChanged -= Object_PropertyChanged;

            InitializeCollections();
        }

        private void Update(TObject o)
        {
            var oldKey = keysByObject[o];
            var newKey = GetKey(o);

            if (!objectsByKey.TryGetValue(newKey, out var newList))
                objectsByKey[newKey] = newList = new List<TObject>();
            
            objectsByKey[oldKey].Remove(o);
            keysByObject[o] = newKey;
            newList.Add(o);
        }

        private void InitializeCollections()
        {
            keysByObject = new Dictionary<TObject, TKey>();
            objectsByKey = new Dictionary<TKey, List<TObject>>();
            nullKeyObjects = new List<TObject>();
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update(sender as TObject);
        }
    }
}
