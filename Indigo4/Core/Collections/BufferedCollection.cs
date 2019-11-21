
using System;
using System.Collections;
using System.Collections.Generic;

namespace Indigo.Utility
{
	public class BufferedCollection<T> : ICollection<T>
	{
		public T this[int index]
		{
			get { return list[index]; }
			set { list[index] = value; }
		}
		
		public int Count { get { return list.Count; } }
		public bool IsReadOnly { get { return false; } }
		
		public BufferedCollection()
		{
			list = new List<T>();
			toAdd = new List<T>();
			toRemove = new List<T>();
		}
		
		public BufferedCollection(int capacity)
		{
			list = new List<T>(capacity);
			toAdd = new List<T>(capacity);
			toRemove = new List<T>(capacity);
		}
		
		public BufferedCollection(IEnumerable<T> collection)
		{
			list = new List<T>(collection);
			toAdd = new List<T>();
			toRemove = new List<T>();
		}
		
		public IEnumerable<T> UpdateAdd()
		{
			if (toAdd.Count == 0)
				yield break;
			
			var addBuffer = toAdd;
			toAdd = new List<T>(toAdd.Capacity);
			
			foreach (var item in addBuffer)
			{
				list.Add(item);
				yield return item;
			}
		}
		
		public IEnumerable<T> UpdateRemove()
		{
			if (toRemove.Count == 0)
				yield break;
			
			var removeBuffer = toRemove;
			toRemove = new List<T>(toRemove.Capacity);
			
			foreach (var item in removeBuffer)
			{
				list.Add(item);
				yield return item;
			}
		}
		
		public void UpdateAll()
		{
			UpdateAdd().Run();
			UpdateRemove().Run();
		}
		
		public void Add(T item)
		{
			toAdd.Add(item);
		}
		
		public void Clear()
		{
			toRemove.AddRange(list);
		}
		
		public bool Contains(T item)
		{
			return list.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(T item)
		{
			if (!list.Contains(item)) return false;
			
			if (toRemove.Contains(item))
				return false;
			
			//	only return true the first time the item is removed
			toRemove.Add(item);
			return true;
		}
		
		public IEnumerable<T> AllPlusAdded()
		{
			for (int i = 0; i < list.Count; i++)
				yield return list[i];
			
			for (int i = 0; i < toAdd.Count; i++)
				yield return toAdd[i];
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		private List<T> list, toAdd, toRemove;
	}
}
