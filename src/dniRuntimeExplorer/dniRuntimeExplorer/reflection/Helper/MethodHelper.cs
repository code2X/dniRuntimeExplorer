using System;
using System.Collections.Generic;
using System.Reflection;

namespace dniRumtimeExplorer.Reflection
{
    public class MethodHelper
    {
        /// <summary>
        /// Get type's all methods
        /// </summary>
        public static MethodInfo[] GetAllMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// Get type's instance methods
        /// </summary>
        public static MethodInfo[] GetInstanceMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Get type's static methods
        /// </summary>
        public static MethodInfo[] GetStaicMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Get method's all params to string
        /// </summary>
        public static string GetParamString(MethodInfo method)
        {
            if (method is null)
                return "";

            string str = "";
            str += "(";
            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (i != parameters.Length - 1)
                {
                    str += parameters[i].ParameterType.Name + " " + parameters[i].Name + ",";
                }
                else
                {
                    str += parameters[i].ParameterType.Name;
                }
            }
            str += ")";

            return str;
        }

        public static List<string> GetMethodList(Type type)
        {
            List<string> methodList = new List<string>();
            var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var method in methodInfos)
            {
                string str = "";
                str += method.Name;
                str += GetParamString(method);
                str += ": " + method.ReturnType.Name;

                methodList.Add(str);
            }

            return methodList;
        }
    }
}


