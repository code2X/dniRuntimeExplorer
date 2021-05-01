using System;
using System.Collections.Generic;

namespace dniRumtimeExplorer.Reflection
{
    /// <summary>
    /// get a string to special type value
    /// </summary>
    public class Parser
    {
        public abstract class IParser
        {
            public abstract bool Parse(string inStr, out object outVal);
            public abstract Type Type();
        }

        public class INT : IParser
        {
            public override Type Type()
            {
                return typeof(int);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                int value;
                if (int.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class FLOAT : IParser
        {
            public override Type Type()
            {
                return typeof(float);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                float value;
                if (float.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class BYTE : IParser
        {
            public override Type Type()
            {
                return typeof(byte);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                byte value;
                if (byte.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class DOUBLE : IParser
        {
            public override Type Type()
            {
                return typeof(double);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                double value;
                if (double.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class ULONG : IParser
        {
            public override Type Type()
            {
                return typeof(ulong);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                ulong value;
                if (ulong.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class UINT : IParser
        {
            public override Type Type()
            {
                return typeof(uint);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                uint value;
                if (uint.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class BOOL : IParser
        {
            public override Type Type()
            {
                return typeof(bool);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                bool value;
                if (bool.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class SHORT : IParser
        {
            public override Type Type()
            {
                return typeof(int);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                short value;
                if (short.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class LONG : IParser
        {
            public override Type Type()
            {
                return typeof(long);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                long value;
                if (long.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class STRING : IParser
        {
            public override Type Type()
            {
                return typeof(string);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                outVal = inStr;
                return true;
            }
        }
    }

    public interface IParserFactory
    {
        Parser.IParser GetParser(Type type);
    }

    /// <summary>
    /// Default parser factory to get class to parse string to special type value
    /// </summary>
    public class DefaultParserFactory: IParserFactory
    {
        List<Parser.IParser> parsers;

        public DefaultParserFactory()
        {
            parsers = new List<Parser.IParser>();
            parsers.Add(new Parser.INT());
            parsers.Add(new Parser.FLOAT());
            parsers.Add(new Parser.BYTE());
            parsers.Add(new Parser.DOUBLE());
            parsers.Add(new Parser.ULONG());
            parsers.Add(new Parser.UINT());
            parsers.Add(new Parser.BOOL());
            parsers.Add(new Parser.SHORT());
            parsers.Add(new Parser.LONG());
            parsers.Add(new Parser.STRING());
        }

        public virtual Parser.IParser GetParser(Type type)
        {
            foreach (Parser.IParser parser in parsers)
            {
#if (Version_4_7)
                if (parser.Type() == type)
//Framework 3.5 occur error when compare type in direct
#else
                if (parser.Type().FullName == type.FullName)
#endif
                {
                    return parser;
                }
            }

            return null;
        }
    }
}
