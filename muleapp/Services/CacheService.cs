using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Runtime.CompilerServices;

namespace Mule
{
    public interface CacheValue {
        string Name { get; }
        object Value(IMemoryCache cache, object instance);
    }

    public class CacheValue<M, T> : CacheValue
        where M : class
    {
        public string Name { get; private set; }

        public CacheValue([CallerMemberName] string name = "")
        {
            Name = $"{typeof(M).FullName}.{name}"; //Capture the type and name of the calling field for use as a cache key
        }

        public Func<M, T> Update { get; set; }

        /// <summary>
        /// Cache key is TypeName.FieldName:instance
        /// </summary>
        public string GetKey(object instance)
        {
            return string.Format("{0}:{1:X}", Name, instance.GetHashCode());
        }

        /// <summary>
        /// Retrieves instance's value from cache or updates the cached value and returns nothing
        /// </summary>
        public object Value(IMemoryCache cache, object instance)
        {
            string key = GetKey(instance);

            object result;
            if (!cache.TryGetValue(key, out result)) //Load the value from the cache if it exists
            {
                try
                {
                    result = Update(instance as M);
                    cache.Set(key, result); //Save the updated value
                }
                catch(Exception ex)
                {
                    cache.Set(key, ex);
                }

            }
            return result;
        }
    }
}
