using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Indigo.Utility
{
    public enum SingletonCollision
    {
        Throw,
        KeepOld,
        OverwriteWithNew
    }

    public class SingletonPropertyHash<TKey, TObject> : IEnumerable<TObject>
        where TObject : class, INotifyPropertyChanged
    {
        public SingletonCollision Behavior { get; }

        public int Count => keysByObject.Count;

        public IEnumerable<TKey> Keys => objectsByKey
            .Select(kv => kv.Key);

        public SingletonPropertyHash(Func<TObject, TKey> getKey, SingletonCollision behavior)
        {
            GetKey = getKey;
            InitializeCollections();
            Behavior = behavior;
        }

        public TObject this[TKey key] => Get(key);
        public bool TryGetValue(TKey key, out TObject value) => objectsByKey.TryGetValue(key, out value);

        public TObject Get(TKey key)
        {
            if (objectsByKey.TryGetValue(key, out var result))
                return result;

            throw new KeyNotFoundException();
        }


        public void Add(TObject o)
        {
            o.PropertyChanged += Object_PropertyChanged;

            var key = GetKey(o);
            if (objectsByKey.TryGetValue(key, out _) && Behavior != SingletonCollision.OverwriteWithNew)
            {
                if (Behavior == SingletonCollision.Throw)
                    throw new Exception($"Attempted to add new value with duplicate key {key}");
            }
            else
            {
                keysByObject[o] = key;
                objectsByKey[key] = o;
            }
        }
        
        public void Remove(TObject o)
        {
            o.PropertyChanged -= Object_PropertyChanged;

            var key = GetKey(o);
            keysByObject.Remove(o);
            objectsByKey.Remove(key);
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

            if (newKey == null || Behavior == SingletonCollision.KeepOld)
            {
                objectsByKey.Remove(oldKey);
                keysByObject.Remove(o);
            }
            else if (objectsByKey.TryGetValue(newKey, out _) && Behavior != SingletonCollision.OverwriteWithNew)
            {
                if (Behavior == SingletonCollision.Throw)
                    throw new Exception($"Attempting to update value caused a collision with key {newKey}");
            }
            else
            {
                objectsByKey.Remove(oldKey);
                keysByObject.Remove(o);
                objectsByKey[newKey] = o;
                keysByObject[o] = newKey;
            }
        }

        private void InitializeCollections()
        {
            keysByObject = new Dictionary<TObject, TKey>();
            objectsByKey = new Dictionary<TKey, TObject>();
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update(sender as TObject);
        }

        public IEnumerator<TObject> GetEnumerator() => objectsByKey.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Dictionary<TObject, TKey> keysByObject;
        private Dictionary<TKey, TObject> objectsByKey;
        private Func<TObject, TKey> GetKey;

    }
}
