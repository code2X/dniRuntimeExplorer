using System;
using System.Reflection;

namespace dniRumtimeExplorer.Reflection
{
    public class FieldHelpers
    {
        /// <summary>
        /// Get type's all fields
        /// </summary>
        public static FieldInfo[] GetAllFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// Get type's all instance fields
        /// </summary>
        public static FieldInfo[] GetInstanceFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Get type's all static fields
        /// </summary>
        public static FieldInfo[] GetStaticFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }
    }
}


