using dniRumtimeExplorer.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace dniRumtimeExplorer.Utils
{
    using ClusterSubDict = SortedDictionary<string, Type>;
    using ClusterDict = SortedDictionary<string, SortedDictionary<string, Type>>;

    /// <summary>
    /// 对程序集中的Class进行分类
    /// </summary>
    public class ClassCluster
    {
        static ClusterSubDict NewClassDict() => new ClusterSubDict();

        /// <summary>
        /// 主要的Class
        /// </summary>
        public static ClusterDict MainClass(ClusterSubDict classDict)
        {
            var classCluster = new ClusterDict();
            classCluster.Add("Enum##Class", NewClassDict());
            classCluster.Add("______##Class", NewClassDict());

            //PreProcess UI And Enum
            foreach (var str2type in classDict)
            {              
                if (str2type.Value.IsEnum)
                {
                    classCluster["Enum##Class"].Add(str2type.Key, str2type.Value);
                }
                classCluster["______##Class"].Add(str2type.Key, str2type.Value);
            }

            RootClass(ref classCluster, classDict);
            SingletonClass(ref classCluster, classDict);

            return classCluster;
        }

        /// <summary>
        /// 自动对Class的名字进行分类
        /// </summary>
        public static ClusterDict ClassCategory(ClusterSubDict classDict)
        {
            var classCluster = new ClusterDict();
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
        /// 命名空间归类
        /// </summary>
        public static ClusterDict NamespaceClasses(ClusterSubDict classDict)
        {
            var classCluster = new ClusterDict();

            classCluster.Add("________##", NewClassDict());

            foreach (var name2type in classDict)
            {
                //主命名空间
                if(name2type.Value.Namespace == null)
                {
                    classCluster["________##"].Add(name2type.Key, name2type.Value);
                }
                else
                {
                    if (classCluster.ContainsKey(name2type.Value.Namespace) == false)
                    {
                        classCluster.Add(name2type.Value.Namespace, NewClassDict());
                    }

                    classCluster[name2type.Value.Namespace].Add(name2type.Key, name2type.Value);
                }

            }

            return classCluster;
        }

        static void SingletonClass(
            ref ClusterDict classCluster,
            ClusterSubDict allClass)
        {
            classCluster.Add("Singleton##Class", NewClassDict());

            bool added = false;

            foreach (var class2type in allClass)
            {
                added = false;

                if (class2type.Value.IsEnum)
                    continue;

                PropertyInfo[] propertyInfos = PropertyHelpers.GetStaticPropertys(class2type.Value);
                FieldInfo[] fieldInfos = FieldHelpers.GetStaticFields(class2type.Value);

                //Seach propertys singleton
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if(propertyInfo.PropertyType.FullName == class2type.Value.FullName)
                    {
                        classCluster["Singleton##Class"].Add(class2type.Key, class2type.Value);
                        added = true;
                        break;
                    }
                }

                if (added)
                    continue;

                //Seach fields singleton
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.FieldType.FullName == class2type.Value.FullName)
                    {
                        classCluster["Singleton##Class"].Add(class2type.Key, class2type.Value);
                        break;
                    }
                }
            }
        }

        static void RootClass(
            ref ClusterDict classCluster,
            ClusterSubDict allClass,
            uint limit = 5
            )
        {
            classCluster.Add("Root##Class", NewClassDict());

            foreach(var class2type in allClass)
            {
                int num = CountClassInclude(class2type.Value, allClass);
                if(num > limit)
                {
                    classCluster["Root##Class"].Add(class2type.Key, class2type.Value);
                }
            }
        }

        /// <summary>
        /// 统计Class的引用和静态成员
        /// </summary>
        static int CountClassInclude(
            Type type,
            ClusterSubDict allClass
        )
        {
            HashSet<Type> types = new HashSet<Type>();
            int bias = 0;     //

            
            PropertyInfo[] propertyInfos = PropertyHelpers.GetAllPropertys(type);
            FieldInfo[] fieldInfos = FieldHelpers.GetAllFields(type);

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
            ref ClusterDict classCluster,
            SortedDictionary<string, Type> allClass,
            SortedDictionary<string, T> classDict
            )
        {
            Dictionary<string, int> startNameCount = CountStartName(classDict);

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
            ref ClusterDict classCluster,
            SortedDictionary<string, Type> allClass,
            SortedDictionary<string, T> classDict
            )
        {
            Dictionary<string, int> endNameCount = CountEndName(classDict);

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

        static Dictionary<string, int> CountStartName<T>(SortedDictionary<string, T> dict)
        {
            Dictionary<string, int> startNameCount = new Dictionary<string, int>();

            foreach (var i in dict)
            {
                string word = FristWord(i.Key);
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

        static Dictionary<string, int> CountEndName<T>(SortedDictionary<string, T> dict)
        {
            Dictionary<string, int> endNameCount = new Dictionary<string, int>();

            foreach (var i in dict)
            {
                string word = EndWord(i.Key);
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

        static string FristWord(string word)
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

        static string EndWord(string word)
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
