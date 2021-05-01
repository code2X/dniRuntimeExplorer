using System;
using ImGuiNET;

namespace dniRumtimeExplorer.ClassViews
{
    public abstract class IClassInfoView
    {
        public static ImGuiTableFlags TableFlags = 
            ImGuiTableFlags.SizingFixedFit | 
            ImGuiTableFlags.Borders | 
            ImGuiTableFlags.Resizable | 
            ImGuiTableFlags.Reorderable | 
            ImGuiTableFlags.Hideable;


        protected bool m_bShow = false;
        Type m_CurrentClassType;
        object m_CurrentClassInstance;

        protected abstract void Draw();
        public virtual void OnGui()
        {
            if (m_bShow)
            {
                Draw();
            }
        }

        public virtual void ShowTypeView(Type type, object instance = null)
        {
            m_CurrentClassType = type;
            ClassName = type.FullName;
            m_CurrentClassInstance = instance;
            m_bShow = true;
        }

        public string ClassName { get;protected set; } = "";
        public virtual Type ClassType => m_CurrentClassType;
        public virtual object ClassInstance => m_CurrentClassInstance;

    }
}
