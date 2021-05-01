using ImGuiNET;
using System.Reflection;
using System;

namespace dniRumtimeExplorer
{
    public abstract class ValueInputWindowBase : IParamInputModalView
    {
        public override string GetPopupName() => "Set Value";
        protected object m_FieldInstance = null;
        protected bool m_Errored = false;
        protected string m_InputText = "";

        public void doSuccess()
        {
            CloseWindow();
        }

        public void DrawTableWithSingleRow(string tableName, Type type, string name, bool errored = false)
        {
            ImGuiView.TableView(tableName, () =>
            {
                ImGui.TableNextRow();
                paramTable.DrawRow(type, name, ref m_InputText, errored);
            }, "Type", "Name", "Value", "Error");
        }

        public void Reset()
        {
            CloseWindow();
            m_FieldInstance = null;
            m_Errored = false;
            m_InputText = "";
        }
    }
}
