
namespace dniRumtimeExplorer.Utils
{
    public class Assert
    {
        public static void IsNotNull(object obj)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(obj is null == false);
#endif
        }

        public static void IsNull(object obj)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(obj is null);
#endif
        }

        public static void IsTrue(object obj)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert((bool)obj is true);
#endif
        }

        public static void IsFalse(object obj)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert((bool)obj is false);
#endif
        }
    }
}
