using ImGuiNET;
using System.Reflection;
using System;
using dniRumtimeExplorer.Reflection;

namespace dniRumtimeExplorer
{
    public class PropertyValueInputWindow : ValueInputWindowBase
    {
        PropertyInfo propertyInfo;

        public PropertyValueInputWindow() { Reset(); }

        public new void Reset()
        {
            base.Reset();
            this.propertyInfo = null;
        }

        public void Show(PropertyInfo propertyInfo, object parentObj = null)
        {
            Reset();
            Utils.Caller.Try(() =>
            {
                this.m_InputText = propertyInfo.GetValue(parentObj, null).ToString();
            });
            ShowWindow();
            this.propertyInfo = propertyInfo;
            this.m_FieldInstance = parentObj;
        }

        public override void DrawPopupContent()
        {
            DrawTableWithSingleRow("propertyValueInputTable", propertyInfo.PropertyType, propertyInfo.Name, m_Errored);

            if (ImGui.Button("OK"))
            {
                m_Errored = !PropertyValueSetter.TrySetValue(propertyInfo, m_InputText, m_FieldInstance);
                if (m_Errored == false)
                {
                    doSuccess();
                }
            }
        }

    }
}
