using dniRumtimeExplorer.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace dniRumtimeExplorer.Reflection
{
    class AttributeTools
    {
        /// <summary>
        /// Get current namespace type with custom attribute type
        /// </summary>
        public static Dictionary<Type, Attribute> GetCustomAttributes(Type attributeType)
        {
            Dictionary<Type, Attribute> type2attr = new Dictionary<Type, Attribute>();

            Type[] namespaceTypes = Assembler.GetNamespaceTypes();
            foreach (Type type in namespaceTypes)
            {
                Attribute attr = Attribute.GetCustomAttribute(type, attributeType);
                if (attr is null == false)
                {
                    type2attr.Add(type, attr);
                }
            }

            return type2attr;
        }

        /// <summary>
        /// Get attribut in class where type is T
        /// </summary>
        public static T GetCustomAttribute<T>(MethodInfo methodInfo) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(methodInfo, typeof(T));
        }

        public static List<T> GetCustomAttributeClassList<T>(Type attributeType)
        {
            List<T> classList = new List<T>();
            var attrList = GetCustomAttributes(attributeType);
            foreach (var pair in attrList)
            {
                Caller.Try(() =>
                {
                    Type type = pair.Key;

                    T containerClass = (T)ClassHelper.New(type);
                    classList.Add(containerClass);
                });
            }

            return classList;
        }
    }
}


