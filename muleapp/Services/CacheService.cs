using Chroniton.Jobs;
using Chroniton.Schedules;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Runtime.CompilerServices;

namespace Mule
{
    /// <summary>
    /// Cache the result of the method and retrieve the result on subsequent calls
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CachedAttribute : Attribute
    {
        public string Name { get; private set; }
        public int ExpireSeconds { get; set; } = 60;
        public TimeSpan Expires { get; set; }
        public CachedAttribute( [CallerMemberName]string name = "")
        {
            Expires = new TimeSpan(0, 0, ExpireSeconds);
            Name = name; //Capture the type and name of the decorated method for use as a cache key
        }

        /// <summary>
        /// Cache key is TypeName.FieldName:instance
        /// </summary>
        public string GetKey<M>(M instance)
        {
            return string.Format("{0}.{1}:{2:X}",instance.GetType().FullName,Name, instance.GetHashCode());
        }

        public virtual T Value<T>(IMemoryCache cache, Delegate update)
        {
            T result;
            if (!cache.TryGetValue(GetKey(update.Target), out result)) //Load the value from the cache if it exists
            {
                result = TryUpdate<T>(cache, update);
            }
            return result;
        }

        public T TryUpdate<T>(IMemoryCache cache, Delegate update)
        {
            string key = GetKey(update.Target);
            try
            {
                return (T)cache.Set(key, update.DynamicInvoke(null), Expires); //Save the updated value
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }

    /// <summary>
    /// Evaluate the method on a background worker and retrieve the result from the cache
    /// </summary>
    public class BackgroundAttribute : CachedAttribute
    {
        public BackgroundAttribute([CallerMemberName]string name = "") : base(name)
        {
            Interval = new TimeSpan(0, 0, IntervalSeconds);
        }

        public int IntervalSeconds { get; set; } = 6;
        public TimeSpan Interval { get; set; }

        public override T Value<T>(IMemoryCache cache, Delegate update)
        {
            T result;
            if (!cache.TryGetValue(GetKey(update.Target), out result)) //If the value already exists, don't start new job
            {
                Chroniton.Singularity.Instance.ScheduleJob(new EveryXTimeSchedule(Interval), new SimpleJob(
                    time => TryUpdate<T>(cache, update)
                ), runImmediately: true); //Start background job
            }
            return result;
        }
    }
}
