//#define Version_4_7
using System;
using System.Reflection;
using dniRumtimeExplorer.Utils;

namespace dniRumtimeExplorer.Reflection
{
    public class ValueSetter
    {
        static DefaultParserFactory parserFactory = new DefaultParserFactory();

        public static bool Parse(Type type,string InputText, out object outVal)
        {
            outVal = null;
            var parser = parserFactory.GetParser(type);
            if (parser == null)
                return false;

            if (parser.Parse(InputText, out outVal))
                return true;
            else
                return false;
        }
    }

    public class FieldValueSetter: ValueSetter
    {
        public static bool SetValue(FieldInfo fieldInfo, string inputText, object instance = null)
        {
#if(Version_4_7)
            if (fieldInfo == null || fieldInfo.IsLiteral)
                return false;
#else
            if (fieldInfo.IsLiteral)
                return false;
#endif
            if (instance == null && fieldInfo.IsStatic == false)
                return false;

            object outVal;
            bool res = Parse(fieldInfo.FieldType, inputText, out outVal);
            if (res == true)
            {
                res = Caller.Try(() =>
                {
                    fieldInfo.SetValue(instance, outVal);
                });
            }
            return res;
        }

        public static bool TrySetValue(FieldInfo fieldInfo, object inputObj, object parent = null)
        {
            bool res = Caller.Try(() =>
            {
                fieldInfo.SetValue(parent, inputObj);
            });
            return res;
        }
    }

    public class PropertyValueSetter: ValueSetter
    {
        public static bool TrySetValue(PropertyInfo propertyInfo, string inputText, object instance = null)
        {
#if (Version_4_7)
            if (propertyInfo == null || propertyInfo.CanWrite == false)
                return false;
#else
            if (propertyInfo.CanWrite == false)
                return false;
#endif
            if (instance == null && 
                propertyInfo.GetAccessors().Length > 0 &&
                propertyInfo.GetAccessors()[0].IsStatic == false)
                return false;

            object outVal;
            bool res = Parse(propertyInfo.PropertyType, inputText, out outVal);
            if (res == true)
            {
                res = Caller.Try(() =>
                {
                    propertyInfo.SetValue(instance, outVal, null);
                });
            }
            return res;
        }

        public static bool TrySetValue(PropertyInfo propertyInfo, object inputObj, object parent = null)
        {
            if (propertyInfo.CanWrite == false)
                return false;

            bool res = Caller.Try(() =>
            {
                propertyInfo.SetValue(parent, inputObj,null);
            });
            return res;
        }
    }

    public class MethodInvoker
    {
        DefaultParserFactory parserFactory = new DefaultParserFactory();
        MethodInfo method;
        object target;    

        public MethodInvoker(MethodInfo method, object target)
        {
            this.method = method;
            this.target = target;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="res">1.Parse error return row num 2.Success return true 3.Invoke error return -1</param>
        /// <param name="strParams"></param>
        /// <returns></returns>
        public int Invoke(out object res,params string[] strParams)
        {
            res = null;
#if (Version_4_7)
            if (this.method == null)
                return -1;
#endif
            if (this.method.IsStatic == false && this.target == null)
                return -1;
            try
            {
                object[] parameters = new object[strParams.Length];
                for(int i = 0; i< strParams.Length; ++i)
                {
                    //GetParser
                    Parser.IParser parser = parserFactory.GetParser(method.GetParameters()[i].ParameterType);
                    if (parser == null)
                        return i + 1;

                    //Parse
                    object outVal;
                    if (parser.Parse(strParams[i],out outVal))
                        parameters[i] = outVal;
                    else
                        return i + 1;
                }

                res = method.Invoke(target, parameters);
                return 0;
            }
            catch(Exception exp)
            {
                Logger.Error(exp);
                return -1;
            }
        }

    }

    public class ArrayElementSetter
    {
        public static bool TrySetValue(Array array, Type type, int index, string text)
        {
#if (Version_4_7)
            if (array == null || type == null)
                return false;
#endif
            if (index >= array.Length || index < 0)
                return false;

            object parsedValue;
            if (ValueSetter.Parse(type, text, out parsedValue))
            {
                bool res = Caller.Try(() =>
                {
                    array.SetValue(parsedValue, index);
                });
                return res;
            }
            else
            {
                return false;
            }
        }
    }


}
