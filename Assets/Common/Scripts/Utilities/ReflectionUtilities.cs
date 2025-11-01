using System;
using System.Collections.Generic;
using System.Linq;

namespace KarenKrill.Utilities
{
    public static class ReflectionUtilities
    {
        public static IEnumerable<Type> GetInheritorTypes(Type interfaceType, params Type[] excludeTypes)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();
                foreach (var type in assemblyTypes)
                {
                    if (!type.IsAbstract && !type.IsInterface && interfaceType.IsAssignableFrom(type) && !excludeTypes.Contains(type))
                    {
                        yield return type;
                    }
                }
            }
        }
    }
}
