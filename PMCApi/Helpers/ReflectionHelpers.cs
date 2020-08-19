using System;
using System.Reflection;

namespace Test.Helpers
{
    public static class ReflectionHelpers
    {
        public static void CopyProperties(object objSource, object objDestination)
        {
            foreach (var propertyInfo in objSource.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(objDestination);
                
                if (string.IsNullOrEmpty(value?.ToString()) ||
                    value?.ToString() == GetDefault(value.GetType())?.ToString()) continue;
                propertyInfo.SetValue(objSource, value);

            }
        }
        public static object GetDefault(this Type type)
        {
            return type.IsValueType ? (!type.IsGenericType ? Activator.CreateInstance(type) : type.GenericTypeArguments[0].GetDefault()) : null;
        }
    }
}