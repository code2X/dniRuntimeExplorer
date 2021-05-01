using dniRumtimeExplorer.Drawer;
using dniRumtimeExplorer.Reflection;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace dniRumtimeExplorer.ClassViews
{
    public class MethodView : IClassInfoView
    {
        public bool Invokeable(MethodInfo method) => method.IsStatic == true || ClassInstance != null;
        public DefaultMethodDrawer methodDrawer = new DefaultMethodDrawer();

        List<MethodInfo> m_InstanceMethods = new List<MethodInfo>();
        List<MethodInfo> m_StaticMethods = new List<MethodInfo>();
        string m_ClassName = "";

        int Comparison(MethodInfo left, MethodInfo right) => left.Name.CompareTo(right.Name);

        public override void ShowTypeView(Type type, object instance = null)
        {
            if (type is null)
                return;

            m_ClassName = type.FullName;

            m_InstanceMethods = new List<MethodInfo>(MethodHelper.GetInstanceMethods(type));
            m_InstanceMethods.Sort(Comparison);

            m_StaticMethods = new List<MethodInfo>(MethodHelper.GetStaicMethods(type));
            m_StaticMethods.Sort(Comparison);

            base.ShowTypeView(type, instance);
        }

        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Method##" + m_ClassName))
            {
                DrawTable(m_InstanceMethods, "ClassMethod##" + m_ClassName);
            }
            if (ImGui.CollapsingHeader("Static Method##" + m_ClassName))
            {              
                DrawTable(m_StaticMethods, "InstanceClassMethod##" + m_ClassName);
            }
        }

        public void DrawTable( List<MethodInfo> table, string TableName )
        {
            if (table.Count == 0) return;

            ImGuiView.TableView(TableName, () =>
             {
                 foreach (var method in table)
                 {
                     ImGui.TableNextRow();
                     DrawTableRow(method);
                 }

             }, TableFlags, "Return Type", "Method Name", "Params");
        }

        public void DrawTableRow(MethodInfo method)
        {
            //Return type
            ImGui.TableSetColumnIndex(0);
            methodDrawer.DrawType(method.ReturnType);

            //Method name
            ImGui.TableSetColumnIndex(1);
            methodDrawer.DrawMethodName(method, ClassInstance,ClassType);

            //Method params
            ImGui.TableSetColumnIndex(2);
            methodDrawer.DrawMethodParams(method, ClassInstance);
        }

    }
}
