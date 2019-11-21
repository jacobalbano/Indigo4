using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Utility
{
    /// <summary>
    /// A simple class for basic reflection.
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// Searches all known assemblies for a type and returns that type.
        /// </summary>
        /// <param name="type">The name of the type to search for.</param>
        /// <returns>The type found, or null if no match.</returns>
        public static Type GetTypeFromAllAssemblies(string type)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                var types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; ++j)
                {
                    var t = types[j];
                    if (t.Name == type)
                        return t;
                }
            }

            return null;
        }

        /// <summary>
        /// <para>Searches all known assemblies for a type and returns that type.</para>
        /// <para>This overload checks that the type implements or inherits from the type parameter.</para>
        /// </summary>
        /// <param name="type">The name of the type to search for.</param>
        /// <returns>The type found, or null if no match.</returns>
        public static Type GetTypeFromAllAssemblies<T>(string type)
        {
            if (type == null)
                return null;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                var types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; ++j)
                {
                    var t = types[j];
                    if (t.Name == type && typeof(T).IsAssignableFrom(t))
                        return t;
                }
            }

            return null;
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(bool includeT = false)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                var types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; ++j)
                {
                    var t = types[j];
                    if (typeof(T).IsAssignableFrom(t))
                        yield return t;
                }
            }
        }
    }
}