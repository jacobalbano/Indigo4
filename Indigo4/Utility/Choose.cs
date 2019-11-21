using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indigo.Utility
{
	/// <summary>
	/// Utility class for performing selections on collections.
	/// By Jacob Albano (www.jacobalbano.com)
	/// MIT license (http://choosealicense.com/licenses/mit/)
	/// </summary>
	public class Choose
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="rng">A method that returns a floating-point number between 0 and 1 (inclusive)</param>
		public Choose(Func<double> rng)
		{
			RNG = rng;
		}
		
		/// <summary>
		/// Choose a random item from a list.
		/// </summary>
		/// <param name="list">The list to choose from.</param>
		/// <returns>The chosen item.</returns>
		public T From<T>(IList<T> list)
		{
			var i = (int) (list.Count * RNG());
			return list[i];
		}
		
		/// <summary>
		/// Choose a random item from a series of options.
		/// </summary>
		/// <param name="options">A parameter list of options to choose from.</param>
		/// <returns>The chosen item.</returns>
		public T Option<T>(params T[] options)
		{
			var i = (int) (options.Length * RNG());
			return options[i];
		}
		
		/// <summary>
		/// Choose a random character from a string.
		/// </summary>
		/// <param name="input">The string to choose from.</param>
		/// <returns>The chosen character.</returns>
		public string Character(string input)
		{
			var i = (int) (RNG() * input.Length);
			return input.Substring(i, 1);
		}
		
		/// <summary>
		/// Choose a random enum from an enum type.
		/// </summary>
		/// <returns>The chosen enum.</returns>
		public T Enum<T>()
		{
			var enums = (T[]) System.Enum.GetValues(typeof(T));
			return Option(enums);
		}
		
		/// <summary>
		/// Choose a random field of a given type from an object.
		/// </summary>
		/// <param name="instance">The object to pull fields from.</param>
		/// <typeparam name="T">The type of the field to choose.</typeparam>
		/// <returns>The chosen field's value.</returns>
		public T Field<T>(object instance)
		{
			return DoChooseField<T>(instance, BindingFlags.Public | BindingFlags.Instance);
		}
		
		/// <summary>
		/// Choose a random static field of a given type from a class.
		/// </summary>
		/// <typeparam name="TResult">The type of the field to choose.</typeparam>
		/// <typeparam name="TClass">The class to pull fields from.</typeparam>
		/// <returns>The chosen field's value.</returns>
		public TResult StaticField<TResult, TClass>()
		{
			return DoChooseField<TResult>(typeof(TClass), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		}
		
		/// <summary>
		/// Choose a random property of a given type from an object.
		/// </summary>
		/// <param name="instance">The object to pull properties from.</param>
		/// <typeparam name="T">The type of the property to choose.</typeparam>
		/// <returns>The chosen property's value.</returns>
		public T Property<T>(object instance)
		{
			return DoChooseProperty<T>(instance, BindingFlags.Public | BindingFlags.Instance);
		}
		
		/// <summary>
		/// Choose a random static property of a given type from a class.
		/// </summary>
		/// <typeparam name="TResult">The type of the properties to choose.</typeparam>
		/// <typeparam name="TClass">The class to pull properties from.</typeparam>
		/// <returns>The chosen property's value.</returns>
		public TResult StaticProperty<TResult, TClass>()
		{
			return DoChooseProperty<TResult>(typeof(TClass), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		}
		
		/// <summary>
		/// Choose randomly from a list where some options are more likely than others.
		/// </summary>
		/// <param name="options">The list of options.</param>
		/// <param name="weights">
		/// <para>A parameter list of weights, where higher numbers are more likely to be chosen.</para>
		/// <para>The options list and weight list must be the same length.</para></param>
		/// <returns>The chosen option.</returns>
		public T Weighted<T>(IList<T> options, params double[] weights)
		{
			if (weights.Length != options.Count)
				throw new Exception("List count must match weight parameters count!");
			
			var total = weights.Sum();
			var rng = total * RNG();
			var upTo = 0.0;
			
			for (int i = 0; i < options.Count; i++)
			{
				var c = options[i];
				var w = weights[i];
				
				if (upTo + w > rng)
					return c;
				
				upTo += w;
			}
			
			throw new Exception("Never should have come here! {draws sword}");
		}
		
		/// <summary>
		/// Choose a random key from a dictionary where the corresponding value determines its likelyhood of being chosen.
		/// </summary>
		/// <param name="weightedOptions">The dictionary to choose from.</param>
		/// <returns>The chosen option.</returns>
		public T Weighted<T>(IDictionary<T, double> weightedOptions)
		{
			var total = weightedOptions.Values.Sum();
			var rng = total * RNG();
			var upTo = 0.0;
			
			foreach (var pair in weightedOptions)
			{
				var c = pair.Key;
				var w = pair.Value;
				
				if (upTo + w > rng)
					return c;
				
				upTo += w;
			}
			
			throw new Exception("No option selected, make sure your weights sum to > 0");
		}
		
		/// <summary>
		/// Choose a random enum from an enum type where some options are more likely than others.
		/// </summary>
		/// <param name="weights">
		/// <para>A parameter list of weights, where higher numbers are more likely to be chosen.</para>
		/// <para>The enum count and weight list must be the same length.</para></param>
		/// <returns></returns>
		public T EnumWeighted<T>(params double[] weights)
		{
			var enums = (T[]) System.Enum.GetValues(typeof(T));
			return Weighted(enums, weights);
		}
		
		/// <summary>
		/// Returns the next item after current in the list of options.
		/// </summary>
		/// <param name="current">The currently selected item (must be one of the options).</param>
		/// <param name="options">An array of all the items to cycle through.</param>
		/// <param name="loop">If true, will jump to the first item after the last item is reached.</param>
		/// <returns>The next item in the list.</returns>
		public T Next<T>(T current, IList<T> options, bool loop = true)
		{
			if (loop) return options[(options.IndexOf(current) + 1) % options.Count];
			return options[Math.Max(options.IndexOf(current) + 1, options.Count - 1)];
		}
		
		/// <summary>
		/// Returns the previous item before current in the list of options.
		/// </summary>
		/// <param name="current">The currently selected item (must be one of the options).</param>
		/// <param name="options">An array of all the items to cycle through.</param>
		/// <param name="loop">If true, will jump to the last item after the first item is reached.</param>
		/// <returns>The next item in the list.</returns>
		public T Prev<T>(T current, IList<T> options, bool loop = true)
		{
			if (loop) return options[((options.IndexOf(current) - 1) + options.Count) % options.Count];
			return options[Math.Max(options.IndexOf(current) - 1, 0)];
		}
		
		#region Quis auxiliis ipos auxilio?
		private T DoChooseField<T>(object obj, BindingFlags flags)
		{
			bool isStatic = IsType(obj);
			Type targetType = isStatic ? (Type) obj : obj.GetType();
			
			var choices = targetType.GetFields(flags)
				.Where(f => f.FieldType == typeof(T)) 
				.Select(f => f.GetValue(obj))
				.Cast<T>()
				.ToArray();
			
			return Option(choices);
		}
		
		private T DoChooseProperty<T>(object obj, BindingFlags flags)
		{
			bool isStatic = IsType(obj);
			Type targetType = isStatic ? (Type) obj : obj.GetType();
			
			var choices = targetType.GetProperties(flags)
				.Where(p => p.CanRead && p.PropertyType == typeof(T))
				.Select(p => p.GetValue(obj, null))
				.Cast<T>()
				.ToArray();
			
			return Option(choices);
		}
		
		private static bool IsType(object target)
		{
			var type = target.GetType();
			var baseType = typeof(Type);
			
			if(type == baseType)
				return true;
			
			var rootType = typeof(object);
			
			while( type != null && type != rootType )
			{
				var current = type.IsGenericType && baseType.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type;
				if( baseType == current )
					return true;
				type = type.BaseType;
			}
			
			return false;
		}
		#endregion
		
		private Func<double> RNG;
	}
}
