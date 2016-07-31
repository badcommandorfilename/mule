using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Mule
{
    /// <summary>
    /// Encloses the state of a given object's property
    /// </summary>
    public class PropertyValue
    {
        readonly PropertyInfo p;
        readonly object i;

        public PropertyValue(PropertyInfo property, object item)
        {
            p = property;
            i = item;
        }

        public string Name { get { return p.Name; } }

        public object Value { get { return p.GetValue(i) ?? ""; } }

        public IEnumerable<Attribute> Attributes { get { return p.GetCustomAttributes(); } }
    }

    /// <summary>
    /// Utilities to automatically inspect or display the properties of models
    /// </summary>
    public static class ModelService
    {
        public static string ToJSON(object model)
        {
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// Returns information about the public properties of a given Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(Type type)
        {
            //For collections (Arrays and Enumerables), get the properties of the inner type
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var innertype = from i in type.GetInterfaces()
                                where i.IsConstructedGenericType
                                && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                select i.GetGenericArguments()[0];
                return innertype.First().GetProperties();
            }
            //For normal types, get the public properties
            return type.GetProperties();
        }

        /// <summary>
        /// Returns information and values of the public properties of a given type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyValue> GetPropertyValues(object model)
        {
            foreach(var p in GetPropertyInfo(model.GetType()))
            {
                yield return new PropertyValue(p, model);
            }
        }
    }
}
