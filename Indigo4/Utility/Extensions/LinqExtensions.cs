using System;
using System.Collections.Generic;

public static class LinqExtensions
{
	public static void Run<T>(this IEnumerable<T> collection)
	{
		var e = collection.GetEnumerator();
		while (e.MoveNext());
	}
}