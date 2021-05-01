using System;
using System.Collections.Generic;
using System.Reflection;

namespace dniRumtimeExplorer.Utils
{
    /// <summary>
    /// 对程序集中的Class进行分类
    /// </summary>
    public class ClassCluster
    {
        static SortedDictionary<string, Type> NewClassDict() => new SortedDictionary<string, Type>();

        /// <summary>
        /// 主要的Class
        /// </summary>
        public static SortedDictionary<string, SortedDictionary<string, Type>> MainCluster(SortedDictionary<string, Type> classDict)
        {
            var classCluster = new SortedDictionary<string, SortedDictionary<string, Type>>();
            classCluster.Add("Enum Class", NewClassDict());
            classCluster.Add("All Class", NewClassDict());

            //PreProcess UI And Enum
            foreach (var str2type in classDict)
            {              
                if (str2type.Value.IsEnum)
                {
                    classCluster["Enum Class"].Add(str2type.Key, str2type.Value);
                }
                classCluster["All Class"].Add(str2type.Key, str2type.Value);
            }

            RootClassCluster(ref classCluster, classDict);

            return classCluster;
        }

        /// <summary>
        /// 自动对Class的名字进行分类
        /// </summary>
        public static SortedDictionary<string, SortedDictionary<string, Type>> AutoCluster(SortedDictionary<string, Type> classDict)
        {
            var classCluster = new SortedDictionary<string, SortedDictionary<string, Type>>();
            var OtherDict = NewClassDict();

            foreach (var i in classDict)
            {
                if (i.Value.IsEnum == false)
                {
                    OtherDict.Add(i.Key, i.Value);
                }
            }

            StartNameCluster(ref classCluster, classDict, OtherDict);
            EndNameCluster(ref classCluster, classDict, OtherDict);

            return classCluster;
        }

        /// <summary>
        /// 自动对Class的名字进行分类
        /// </summary>
        public static SortedDictionary<string, SortedDictionary<string, Type>> NamespaceCluster(SortedDictionary<string, Type> classDict)
        {
            var classCluster = new SortedDictionary<string, SortedDictionary<string, Type>>();
            var OtherDict = NewClassDict();

            foreach (var name2type in classDict)
            {
                if(classCluster.ContainsKey(name2type.Value.Namespace) == false )
                {
                    classCluster.Add(name2type.Value.Namespace, NewClassDict());
                }

                classCluster[name2type.Value.Namespace].Add(name2type.Key, name2type.Value);
            }

            return classCluster;
        }

        static void RootClassCluster(
            ref SortedDictionary<string, SortedDictionary<string, Type>> classCluster,
            SortedDictionary<string, Type> allClass,
            uint limit = 5
            )
        {
            classCluster.Add("Root Class", NewClassDict());

            foreach(var class2type in allClass)
            {
                int num = CountClassInclude(class2type.Value, allClass);
                if(num > limit)
                {
                    classCluster["Root Class"].Add(class2type.Key, class2type.Value);
                }
            }
        }

        /// <summary>
        /// 统计Class的引用和静态成员
        /// </summary>
        static int CountClassInclude(
            Type type,
            SortedDictionary<string, Type> allClass
        )
        {
            HashSet<Type> types = new HashSet<Type>();
            int bias = 0;     //

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            //count field class include number
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Caller.Try(() =>
                {
                    if (allClass.ContainsKey(fieldInfo.FieldType.Name))
                    {
                        //static class field
                        if (fieldInfo.IsStatic && fieldInfo.FieldType.IsEnum == false)
                        {
                            bias += 1;
                        }
                        types.Add(fieldInfo.FieldType);
                    }
                });
            }

            //count property class include number
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (allClass.ContainsKey(propertyInfo.PropertyType.Name))
                {
                    //static class property
                    if (propertyInfo.GetAccessors().Length > 0 && 
                        propertyInfo.GetAccessors()[0].IsStatic &&
                        propertyInfo.PropertyType.IsEnum == false)
                    {
                        bias += 1;
                    }                      
                    types.Add(propertyInfo.PropertyType);
                }
            }

            return types.Count + bias;
        }

        static void StartNameCluster<T>(
            ref SortedDictionary<string, SortedDictionary<string, Type>> classCluster,
            SortedDictionary<string, Type> allClass,
            SortedDictionary<string, T> classDict
            )
        {
            Dictionary<string, int> startNameCount = StartNameCount(classDict);

            foreach (var i in startNameCount)
            {
                if (i.Value > 4 &&
                    i.Key.Length > 1 &&
                    classCluster.ContainsKey(i.Key) == false)
                {
                    classCluster.Add(i.Key, new SortedDictionary<string, Type>());
                    foreach (var str2type in allClass)
                    {
                        if (str2type.Key.StartsWith(i.Key))
                        {
                            classCluster[i.Key].Add(str2type.Key, str2type.Value);
                        }
                    }
                }
            }
        }

        static void EndNameCluster<T>(
            ref SortedDictionary<string, SortedDictionary<string, Type>> classCluster,
            SortedDictionary<string, Type> allClass,
            SortedDictionary<string, T> classDict
            )
        {
            Dictionary<string, int> endNameCount = EndNameCount(classDict);

            foreach (var i in endNameCount)
            {
                if (i.Value > 4 &&
                    i.Key.Length > 1 &&
                    classCluster.ContainsKey(i.Key) == false)
                {
                    classCluster.Add(i.Key, new SortedDictionary<string, Type>());
                    foreach (var str2type in allClass)
                    {
                        if (str2type.Key.EndsWith(i.Key))
                        {
                            classCluster[i.Key].Add(str2type.Key, str2type.Value);
                        }
                    }
                }
            }
        }

        static Dictionary<string, int> StartNameCount<T>(SortedDictionary<string, T> dict)
        {
            Dictionary<string, int> startNameCount = new Dictionary<string, int>();

            foreach (var i in dict)
            {
                string word = fristWord(i.Key);
                if (startNameCount.ContainsKey(word))
                {
                    ++startNameCount[word];
                }
                else
                {
                    startNameCount[word] = 1;
                }
            }

            return startNameCount;
        }

        static Dictionary<string, int> EndNameCount<T>(SortedDictionary<string, T> dict)
        {
            Dictionary<string, int> endNameCount = new Dictionary<string, int>();

            foreach (var i in dict)
            {
                string word = endWord(i.Key);
                if (endNameCount.ContainsKey(word))
                {
                    ++endNameCount[word];
                }
                else
                {
                    endNameCount[word] = 1;
                }
            }

            return endNameCount;
        }

        static string fristWord(string word)
        {
            if (word.Length > 2)
            {
                for (int i = 1; i < word.Length; ++i)
                {
                    if ('A' <= word[i] && 'Z' >= word[i])
                    {
                        return word.Substring(0, i);
                    }
                }
            }
            return word;
        }

        static string endWord(string word)
        {
            for (int i = word.Length - 1; i >= 0; --i)
            {
                if ('A' <= word[i] && 'Z' >= word[i])
                {
                    return word.Substring(i, word.Length - i);
                }
            }
            return word;
        }
    }
}
