using dniRumtimeExplorer.Drawer;
using dniRumtimeExplorer.Reflection;
using dniRumtimeExplorer.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace dniRumtimeExplorer.ClassViews
{
    class FieldView : IClassInfoView
    {
        bool FieldReadable(FieldInfo field) => ClassInstance != null || field.IsStatic;

        public DefaultFieldDrawer m_FieldDrawer = new DefaultFieldDrawer();

        List<FieldInfo> m_InstanceFields = new List<FieldInfo>();
        List<FieldInfo> m_StaticFields = new List<FieldInfo>();
        string m_ClassName = "";

        int Comparison(FieldInfo left, FieldInfo right) => left.Name.CompareTo(right.Name);

        public override void ShowTypeView(Type type, object instance = null)
        {
            if (type is null)
                return;

            m_ClassName = type.FullName;

            m_InstanceFields = new List<FieldInfo>(FieldHelpers.GetInstanceFields(type));
            m_InstanceFields.Sort(Comparison);

            m_StaticFields = new List<FieldInfo>(FieldHelpers.GetStaticFields(type));
            m_StaticFields.Sort(Comparison);

            base.ShowTypeView(type, instance);
        }

        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Fields##" + m_ClassName))
            {
                DrawFieldTable(m_InstanceFields, "ClassInstanceFields##" + m_ClassName);
            }

            if (ImGui.CollapsingHeader("Static Fields##" + m_ClassName))
            {              
                DrawFieldTable(m_StaticFields, "ClassStaticFields##" + m_ClassName);
            }
        }

        public void DrawFieldTable(List<FieldInfo> fieldList, string tableName)
        {
            if (fieldList.Count == 0) return;

            ImGuiView.TableView(tableName, () =>
            {
                foreach (var field in fieldList)
                {
                    ImGui.TableNextRow();
                    DrawTableRow(field, m_FieldDrawer);
                }

            }, TableFlags, "Type", "Name", "Value");
        }

        public void DrawTableRow(FieldInfo field, IFieldDrawer drawer)
        {
            //Field type
            ImGui.TableSetColumnIndex(0);
            drawer.DrawType(field.FieldType);

            //Field name
            ImGui.TableSetColumnIndex(1);
            Caller.Try(() =>
            {
                object instance = FieldReadable(field) ? field.GetValue(ClassInstance) : null;
                drawer.DrawName(field.Name,field.FieldType,ClassType,instance);
            });

            //Field value
            if(FieldReadable(field))
            {
                ImGui.TableSetColumnIndex(2);
                drawer.DrawFieldValue(field, ClassInstance);
            }
        }

    }
}
