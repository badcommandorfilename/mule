using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

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
        /// Returns information about the public properties and methods of a given Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetMemberInfo(Type type)
        {
            foreach (var p in GetPropertyInfo(type))
            {
                yield return p;
            }

            foreach (var f in 
                from m in ModelType(type).GetMethods()
                where m.GetCustomAttribute<CachedAttribute>() != null
                select m )
            {
                yield return f;
            }
        }

        /// <summary>
        /// Returns information and values of the public properties and methods of a given type
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
                foreach (var f in ModelType(model.GetType()).GetMethods())
                {
                    var a = f.GetCustomAttribute<CachedAttribute>();
                    if(a != null)
                    {
                        var v = a.Value<object>(cache, 
                            f.CreateDelegate(Expression.GetFuncType(f.ReturnType), 
                            model));
                        yield return new KeyValuePair<string, object>(f.Name, v ?? "");
                    }
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

        /// <summary>
        /// Discover all data models in the assembly
        /// Models will have a Primary Key property with
        /// System.ComponentModel.DataAnnotations.KeyAttribute
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> AllModels()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            return from t in asm.GetTypes()
                   where t.GetProperties().Any(p => //Models must have a [Key] declared
                        p.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() != null)
                   select t;
        }
    }
}
