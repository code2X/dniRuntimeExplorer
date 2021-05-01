using ImGuiNET;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace dniRumtimeExplorer
{

    public abstract class IModalView : ImGuiEx.ImGuiExView
    {
        bool showWindow = false;

        public virtual void OnGui()
        {
            if (showWindow)
            {
#if (ModalWindow)
                if (!ImGui.IsPopupOpen(GetPopupName()))
                    ImGui.OpenPopup(GetPopupName());
                if (ImGui.BeginPopupModal(GetPopupName(), ref showWindow))
                {
                    DrawPopupContent();
                    ImGui.EndPopup();
                }
#else
                ImGui.Begin(GetPopupName(), ref showWindow);
                DrawPopupContent();
                ImGui.End();
#endif
            }
        }

        public virtual void ShowWindow() => showWindow = true;
        public virtual void CloseWindow() => showWindow = false;
        public virtual string GetPopupName() => "ModalWindow";
        public abstract void DrawPopupContent();
    }

    public class ParamInputTable
    {
        static Vector4 errorColor = new Vector4(0.4f, 0.1f, 0.1f, 0.65f);

        public void DrawRow(Type type, string name, ref string inputText, bool error = false)
        {
            if (error)
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0 + 1, ImGui.GetColorU32(errorColor));
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(type.Name);

            ImGui.TableSetColumnIndex(1);
            ImGui.Text(name);

            ImGui.TableSetColumnIndex(2);
            ImGui.InputText("##" + name, ref inputText, 20);

            ImGui.TableSetColumnIndex(3);
            ImGui.Text(error.ToString());
        }
    }

    public abstract class IParamInputModalView : IModalView
    {
        public static ParamInputTable paramTable = new ParamInputTable();
    }

}
