using System;
using ImGuiNET;
using System.Reflection;
using dniRumtimeExplorer.Utils;
using dniRumtimeExplorer.Reflection;

namespace dniRumtimeExplorer.Drawer
{
    public class DefaultTypeDrawer : ITypeDrawer
    {
        Vector4 color = new Vector4(0.1f, 0.1f, 0.1f, 0.65f);

        public override void DrawType(Type type)
        {
            if (type is null)
                return;
            
            if (type.IsArray)
            {
                ImGui.Button(type.Name);
            }
            else if (IsGeneralType(type) == false)
            {
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0 + 1, ImGui.GetColorU32(color));
                if (ImGui.Button(type.Name))
                {
                    RuntimeExplorerApp.Instance.ClassWindow.OpenTab(type);
                }
            }
            else
            {
                ImGui.Text(type.Name);
            }
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            if (type is null)
                return;

            Caller.Try(() =>
            {
                if (type.IsArray)
                {

                    if (ImGui.Button(name) && instance != null)
                    {
                        ArrayInfoWindow.GetInstance().Show(instance, name);
                    }
                }
                else if (IsGeneralType(type) == false && type.IsEnum == false)
                {
                    if (ImGui.Button(name) && instance != null)
                    {
                        RuntimeExplorerApp.Instance.ClassWindow.AddNewInstance(name, parent, instance);
                    }
                }
                else
                {
                    ImGui.Text(name);
                }
            });
        }
    }

    public class DefaultArrayDrawer : IArrayDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();

        public override void DrawType(Type type)
        {
            typeDrawer.DrawType(type);
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            typeDrawer.DrawName(name, type, parent, instance);
        }

        public override void DrawArrayIndex(int index) 
        {
            ImGui.Text(index.ToString());
        }

        public override void DrawArrayValue(Array array, object element, int index)
        {
            if (array is null)
                return;

            Caller.Try(() =>
            {
                ImGui.Text(element.ToString());
                ImGui.SameLine();

                if (ImGui.ArrowButton(index.ToString() + element.ToString(), ImGuiDir.Left))
                {
                    ArrayElementInputWindow.GetInstance().Show(array, element, index);
                }
            });
        }
    }

    public class DefaultFieldDrawer : IFieldDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();

        public override void DrawType(Type type)
        {
            typeDrawer.DrawType(type);
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            typeDrawer.DrawName(name, type, parent, instance);
        }

        public override void DrawFieldValue(FieldInfo field, object instance = null)
        {
            if (field is null)
                return;

            if (IsGeneralType(field.FieldType) == false
                && field.FieldType.IsEnum == false)
                return;

            //Can't get value
            if (instance == null
                 && field.IsStatic == false)
                return;

            Caller.Try(() =>
            {
                string value = field.GetValue(instance).ToString();
                if (field.IsLiteral)
                {
                    ImGui.Text(value);
                }
                else
                {
                    ImGui.Text(value);
                    ImGui.SameLine();
                    if ( ImGui.ArrowButton("##" + field.ToString(),ImGuiDir.Down))
                    {
                        RuntimeExplorerApp.Instance.FieldValueInputWindow.Show(field, instance);
                    }
                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip("Set " + field.Name + " value");
                }
            });
        }

    }

    public class DefaultPropertyDrawer : IPropertyDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();

        public bool IsStatic(PropertyInfo property) =>
            property.GetAccessors().Length > 0 && property.GetAccessors()[0].IsStatic;
            
        public bool Invokable(PropertyInfo property, object instance) =>
            instance != null || IsStatic(property) == true;

        public override void DrawType(Type type)
        {
            if (type is null)
                return;

            typeDrawer.DrawType(type);
        }

        public virtual void DrawPropertyName(PropertyInfo property, object classInstance, Type parent)
        {
            if (property is null)
                return;

            object instance = null;
            Caller.Try(() =>
            {
                if (Invokable(property, classInstance))
                {
                    instance = property.CanRead ? property.GetValue(classInstance, null) : null;
                }
            });

            typeDrawer.DrawName(
                property.Name ,
                property.PropertyType,
                parent,
                instance);
        }

        public override void DrawPropertyValue(PropertyInfo property, object classInstance = null)
        {
            if (property is null)
                return;

            if (IsGeneralType(property.PropertyType) == false 
                && property.PropertyType.IsEnum == false)
                return;

            if (property.CanRead == false)
                return;

            if (Invokable(property, classInstance) == false)
                return;

            string value = "";
            Caller.Try(() =>
            {   
                value = property.GetValue(classInstance,null).ToString();
            });

            if (property.CanWrite == false)
            {
                ImGui.Text(value);
            }
            else
            {
                ImGui.Text(value);
                ImGui.SameLine();
                if (ImGui.ArrowButton("##" + property.Name, ImGuiDir.Down))
                    RuntimeExplorerApp.Instance.PropertyValueInputWindow.Show(property, classInstance);
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Set " + property.Name + " value");
            }
        }
    }

    public class DefaultMethodDrawer : IMethodDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();
        public bool CanInvoke(MethodInfo method,object instance) => method.IsStatic == true || instance != null;

        public override void DrawType(Type type)
        {
            typeDrawer.DrawType(type);
        }

        public virtual void DrawMethodName(MethodInfo method, object instance, Type parent)
        {
            if (method is null)
                return;

            string label = MethodHelper.GetParamString(method);

            if (ImGui.Button(method.Name + "##" + label))
            {
                ImGui.OpenPopup(method.Name + "##" + label);
            }

            DrawMethodMenu(method, instance, label);
        }

        void DrawMethodMenu(MethodInfo method, object instance,string label)
        {
            if (method is null)
                return;

            ImGuiView.PopupView(method.Name + "##" + label, () =>
             {
                 if (ImGui.Button("Invoke"))
                 {
                     if (CanInvoke(method, instance))
                     {
                         RuntimeExplorerApp.Instance.MethodInvokeWindow.Show(method, method.GetParameters(), instance);
                     }
                     else
                     {
                         ImGui.CloseCurrentPopup();
                     }
                 }
                 //ImGui.Button("Hook");
             });
        }

        public override void DrawMethodParams(MethodInfo method, object instance = null)
        {
            if (method is null)
                return;

            string paramStr = MethodHelper.GetParamString(method);
            ImGui.Text(paramStr);
        }


    }


}
