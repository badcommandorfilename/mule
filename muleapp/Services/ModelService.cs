using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Mule
{
    /// <summary>
    /// Utilities to automatically inspect or display the properties of models
    /// </summary>
    public static class ModelService
    {
        public static string ToJSON(object model)
        {
            return JsonConvert.SerializeObject(
                GetModelValues(model)
                .ToDictionary(x => x.Key, x => x.Value));
        }

        /// <summary>
        /// Returns information about the public properties of a given Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(Type type)
        {
            return ModelType(type).GetProperties();
        }

        /// <summary>
        /// Returns information about the public properties of a given Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFieldInfo<T>(Type type)
        {
            return ModelType(type).GetFields().Where(f => typeof(T).IsAssignableFrom(f.FieldType));
        }

        /// <summary>
        /// Returns information and values of the public properties of a given type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> GetModelValues(object model, IMemoryCache cache = null)
        {
            foreach(var p in GetPropertyInfo(model.GetType()))
            {
                yield return new KeyValuePair<string, object>(p.Name, p.GetValue(model) ?? "");
            }

            if(cache != null)
            {
                foreach (var f in GetFieldInfo<CacheValue>(model.GetType()))
                {
                    var v = f.GetValue(model);
                    yield return new KeyValuePair<string, object>(f.Name, (v as CacheValue).Value(cache, model) ?? "");
                }
            }

        }

        /// <summary>
        /// Detects model type from object or collection
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ModelType(Type type)
        {
            //For collections (Arrays and Enumerables), get the properties of the inner type
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var innertype = from i in type.GetInterfaces()
                                where i.IsConstructedGenericType
                                && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                select i.GetGenericArguments()[0];
                return innertype.First();
            }
            //For normal types, get the public properties
            return type;
        }

        public static IEnumerable<Type> AllModels()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            return from t in asm.GetTypes()
                   where typeof(Controller).IsAssignableFrom(t)
                   && !t.GetTypeInfo().IsAbstract
                   && t.GetTypeInfo().BaseType.IsConstructedGenericType
                   select t.GetTypeInfo().BaseType.GetGenericArguments()[0];
        }

    }
}
