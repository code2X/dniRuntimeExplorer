using System;
using System.Collections.Generic;

namespace dniRumtimeExplorer.Reflection
{
    class CsharpKeywords
    {
        public static HashSet<Type> GeneralTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(float),
            typeof(byte),
            typeof(double),
            typeof(ulong),
            typeof(uint),
            typeof(bool),
            typeof(Boolean),
            typeof(short),
            typeof(long),
            typeof(string)
        };
    }

}
