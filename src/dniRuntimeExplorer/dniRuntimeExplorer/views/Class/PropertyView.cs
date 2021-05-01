using System;
using ImGuiNET;
using System.Collections.Generic;
using System.Reflection;
using dniRumtimeExplorer.Drawer;
using dniRumtimeExplorer.Reflection;

namespace dniRumtimeExplorer.ClassViews
{
    public class PropertyView : IClassInfoView
    {
        public bool PropertyReadable(PropertyInfo property) => ClassInstance != null ;
        public DefaultPropertyDrawer propertyDrawer = new DefaultPropertyDrawer();

        List<PropertyInfo> m_InstancePropertys = new List<PropertyInfo>();
        List<PropertyInfo> m_StaticPropertys = new List<PropertyInfo>();

        int Comparison(PropertyInfo left, PropertyInfo right) => left.Name.CompareTo(right.Name);

        public override void ShowTypeView(Type type, object instance = null)
        {
            if (type is null)
                return;
            m_InstancePropertys = new List<PropertyInfo>(PropertyHelpers.GetInstancePropertys(type));
            m_InstancePropertys.Sort(Comparison);

            m_StaticPropertys = new List<PropertyInfo>(PropertyHelpers.GetStaticPropertys(type));
            m_StaticPropertys.Sort(Comparison);

            base.ShowTypeView(type, instance);
        }

        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Property"))
            {
                DrawTable(m_InstancePropertys, "##PropertyTable" + ClassName);
            }
            if (ImGui.CollapsingHeader("Static Property"))
            {
                DrawTable(m_StaticPropertys, "##StaticPropertyTable" + ClassName);
            }
        }

        public void DrawTable(
            List<PropertyInfo> propertyList,
            string table_name = "StaticPropertyTable"
            )
        {
            if (propertyList.Count == 0) return;

            ImGuiView.TableView(table_name, () =>
             {
                 foreach (PropertyInfo property in propertyList)
                 {
                     ImGui.TableNextRow();
                     DrawTableRow(property);
                 }

             }, TableFlags, "Type", "Name", "Value", "Gettable", "Settable");
        }

        public void DrawTableRow( PropertyInfo property)
        {
            //Property type
            ImGui.TableNextColumn();
            propertyDrawer.DrawType(property.PropertyType);

            //Property name
            ImGui.TableNextColumn();
            propertyDrawer.DrawPropertyName(property, ClassInstance, ClassType);

            //Propery read value
            ImGui.TableNextColumn();
            propertyDrawer.DrawPropertyValue(property, ClassInstance);

            //Readable
            ImGui.TableNextColumn();
            ImGui.Text(property.CanRead.ToString());

            //Writable
            ImGui.TableNextColumn();
            ImGui.Text(property.CanWrite.ToString());
        }
    }
}
