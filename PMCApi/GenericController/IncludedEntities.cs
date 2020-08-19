using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test.Models
{
    public class IncludedEntities
    {
        public static IReadOnlyList<TypeInfo> Types;

        static IncludedEntities()
        {
            //Get all classes  
            var allTypes = typeof(IncludedEntities).GetTypeInfo().Assembly.GetTypes();
            var typeList = new List<TypeInfo>();

            foreach (Type type in allTypes)
            {   // si la classe possede ApiEntityAttribute comme attribute
                if (type.GetCustomAttributes(typeof(ApiEntityAttribute), true).Length > 0)
                {
                    typeList.Add(type.GetTypeInfo());
                }
            }
            Types = typeList;
        }
    }
}