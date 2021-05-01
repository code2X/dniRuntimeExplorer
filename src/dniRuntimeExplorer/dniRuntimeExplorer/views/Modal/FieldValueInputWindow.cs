using dniRumtimeExplorer.Reflection;
using dniRumtimeExplorer.Utils;
using ImGuiNET;
using System.Reflection;

namespace dniRumtimeExplorer
{
    public class FieldValueInputWindow : ValueInputWindowBase
    {
        FieldInfo m_FieldInfo = null;
        public FieldValueInputWindow() 
        { 
            Reset(); 
        }

        public new void Reset()
        {
            base.Reset();
            m_FieldInfo = null;
        }

        public void Show(FieldInfo fieldInfo, object fieldInstance = null)
        {
            Reset();
            Caller.Try(() =>
            {
                m_InputText = fieldInfo.GetValue(fieldInstance).ToString();
            });
            ShowWindow();
            m_FieldInfo = fieldInfo;
            m_FieldInstance = fieldInstance;
        }

        public override void DrawPopupContent()
        {
            if (m_FieldInfo is null)
                return;

            DrawTableWithSingleRow("FieldValueInputTable", m_FieldInfo.FieldType, m_FieldInfo.Name, m_Errored);

            if (ImGui.Button("OK##FieldValueInputWindow"))
            {
                m_Errored = !FieldValueSetter.SetValue(m_FieldInfo, m_InputText, m_FieldInstance);
                if (m_Errored == false)
                {
                    doSuccess();
                }
            }
        }

    }

}
