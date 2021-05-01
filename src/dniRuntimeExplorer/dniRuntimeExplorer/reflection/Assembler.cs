using System;
using System.Collections.Generic;
using System.Reflection;

namespace dniRumtimeExplorer.Reflection
{
    /// <summary>
    /// Assembly Encapsulation 
    /// </summary>
    public class Assembler
    {
        Assembly m_Assembly = null;

        public Assembler()
        {

        }

        /// <summary>
        /// Load a assembly
        /// </summary>
        public bool Load(string filePath)
        {
            bool res = Utils.Caller.Try(() =>
            {
                m_Assembly = Assembly.LoadFrom(filePath);
                Parse();
            });

            return res;
        }

        internal static Type[] GetNamespaceTypes()
        {
            throw new NotImplementedException();
        }

        List<Type> m_AssemblyTypes = new List<Type>();
        List<Type> m_MainNamespaceTypes = new List<Type>();

        public string Name => m_Assembly.GetName().Name;
        public string Location => m_Assembly.Location;
        public Assembly Assembly => m_Assembly;
        public List<Type> AssemblyTypes => m_AssemblyTypes;
        public List<Type> MainNamespaceTypes => m_MainNamespaceTypes;

        protected virtual void Parse()
        {
            foreach (Type i in m_Assembly.GetExportedTypes())
            {
                AssemblyTypes.Add(i);
                if (i.Namespace is null)
                {
                    MainNamespaceTypes.Add(i);
                }
            }
        }

    }

}



