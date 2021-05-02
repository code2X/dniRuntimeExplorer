using System;
using System.Diagnostics;

namespace dniRumtimeExplorer.Utils
{
    public class Logger
    {
        public static void Error(params string[] errors)
        {
#if DEBUG
            foreach (string err in errors)
            {
                Console.WriteLine("Error: " + err);
            }
#endif
        }

        public static void Info(params string[] infos)
        {
#if DEBUG
            foreach (string info in infos)
            {
                Console.WriteLine("Info: " + info);
            }
#endif
        }

        public static void Warn(params string[] warns)
        {
#if DEBUG
            foreach (string warn in warns)
            {
                Console.WriteLine("Warn: " + warn);
            }
#endif
        }

        public static void Error(Exception exp)
        {
#if DEBUG
            Console.WriteLine("Error: " + exp);

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);     //0为方法本身，1为调用的方法 
            Console.WriteLine(" File: {0}", sf.GetFileName());
            Console.WriteLine(" Method: {0}", sf.GetMethod().Name);
            Console.WriteLine(" Line Number: {0}", sf.GetFileLineNumber());
            Console.WriteLine(" Column Number: {0}", sf.GetFileColumnNumber());
#endif
        }
    }
}
