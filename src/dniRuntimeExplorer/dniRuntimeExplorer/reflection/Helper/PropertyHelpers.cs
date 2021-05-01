using System;
using System.Reflection;

namespace dniRumtimeExplorer.Reflection
{
    public class PropertyHelpers
    {
        /// <summary>
        /// Get type's all fields
        /// </summary>
        public static PropertyInfo[] GetAllPropertys(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// Get type's all instance fields
        /// </summary>
        public static PropertyInfo[] GetInstancePropertys(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Get type's all static fields
        /// </summary>
        public static PropertyInfo[] GetStaticPropertys(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }
    }
}


