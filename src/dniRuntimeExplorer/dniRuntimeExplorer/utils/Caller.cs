using System;
using System.Threading;

public delegate void Callback();

namespace dniRumtimeExplorer.Utils
{
    public class Caller
    {

        /// <summary>
        /// Error return false
        /// </summary>
        public static bool Try(Callback func)
        {
            try
            {
                func();
                return true;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                return false;
            }
        }

        public static void EnterMutex(Mutex mutex, Callback voidFunc)
        {
            if (mutex.WaitOne())
            {
                voidFunc();
                mutex.ReleaseMutex();
            }
        }

        public static void TryEnterMutex(Mutex mutex, Callback voidFunc)
        {
            try
            {
                EnterMutex(mutex, voidFunc);
            }
            catch (Exception exp)
            {
                mutex.ReleaseMutex();
                Logger.Error(exp);
            }
        }
    }
}


