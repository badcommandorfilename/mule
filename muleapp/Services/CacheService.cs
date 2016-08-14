using Chroniton.Jobs;
using Chroniton.Schedules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Runtime.CompilerServices;

namespace Mule
{
    public interface ICacheValue
    {
        string Name { get; }
        object Value(IMemoryCache cache, object instance);
    }

    public class CacheValue<M, T> : ICacheValue
        where M : class
    {
        public string Name { get; private set; }
        public Func<M, T> Update { get; set; }
        public TimeSpan Expires { get; set; } = new TimeSpan(1, 0, 0);

        public CacheValue([CallerMemberName] string name = "")
        {
            Name = $"{typeof(M).FullName}.{name}"; //Capture the type and name of the calling field for use as a cache key
        }


        /// <summary>
        /// Cache key is TypeName.FieldName:instance
        /// </summary>
        public string GetKey(M instance)
        {
            return string.Format("{0}:{1:X}", Name, instance.GetHashCode());
        }

        /// <summary>
        /// Retrieves instance's value from cache or updates the cached value and returns nothing
        /// </summary>
        public T Value(IMemoryCache cache, M instance)
        {
            T result;
            if (!cache.TryGetValue(GetKey(instance), out result)) //Load the value from the cache if it exists
            {
                result = TryUpdate(cache, instance);
            }
            return result;
        }

        public T TryUpdate(IMemoryCache cache, M instance)
        {
            string key = GetKey(instance);
            try
            {
                return cache.Set(key, Update(instance), Expires); //Save the updated value
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        object ICacheValue.Value(IMemoryCache cache, object instance)
        {
            return Value(cache, instance as M);
        }
    }

    public class UpdatingValue<M, T> : CacheValue<M, T>, ICacheValue
    where M : class
    {
        public TimeSpan Interval { get; set; } = new TimeSpan(0, 0, 5);

        public UpdatingValue([CallerMemberName]string name = "") : base(name)
        {
        }

        public new T Value(IMemoryCache cache, M instance)
        {
            T result;
            if (!cache.TryGetValue(GetKey(instance), out result)) //If the value already exists, don't start new job
            {
                Chroniton.Singularity.Instance.ScheduleJob(new EveryXTimeSchedule(Interval), new SimpleJob(
                    time => TryUpdate(cache, instance)
                ), runImmediately: true); //Start background job
            }
            return result;
        }

        object ICacheValue.Value(IMemoryCache cache, object instance)
        {
            return Value(cache, instance as M);
        }
    }
}
