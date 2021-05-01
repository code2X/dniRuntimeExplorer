using System;
using dniRumtimeExplorer.Utils;

namespace dniRumtimeExplorer.Reflection
{
    public class ClassHelper
    {
        /// <summary>
        /// Call type default constructor to get a new instance
        /// </summary>
        public static object New(System.Type type)
        {
            var constructor = type.GetConstructor(System.Type.EmptyTypes);
            Assert.IsNotNull(constructor);
            return constructor.Invoke(null);
        }

        /// <summary>
        /// Call type single param constructor to get a new instance
        /// </summary>
        public static object New<T>(System.Type type, T value)
        {
            Type[] types = new Type[1];
            types[0] = typeof(T);
            object[] param = new object[1];
            param[0] = value;

            var constructor = type.GetConstructor(types);
            Assert.IsNotNull(constructor);
            return constructor.Invoke(param);
        }
    }
}



