using dniRumtimeExplorer.Reflection;
using System;
using System.Reflection;

namespace dniRumtimeExplorer.Drawer
{
    public abstract class ITypeDrawer
    {
        public bool IsGeneralType(Type type) => CsharpKeywords.GeneralTypes.Contains(type);
        public virtual void DrawType(Type type) { }
        public virtual void DrawName(string name, Type type, Type parent, object instance = null) { }
    }

    public abstract class IFieldDrawer : ITypeDrawer
    {
        public virtual void DrawFieldValue(FieldInfo field, object instance = null) { }
    }

    public abstract class IPropertyDrawer : ITypeDrawer
    {
        public virtual void DrawPropertyValue(PropertyInfo property, object instance = null) { }
    }

    public abstract class IMethodDrawer : ITypeDrawer
    {
        public virtual void DrawMethodParams(MethodInfo method, object instance = null) { }
    }

    public abstract class IArrayDrawer : ITypeDrawer
    {
        public virtual void DrawArrayIndex(int index) { }
        public virtual void DrawArrayValue(Array array, object element, int index) { }
    }
}
