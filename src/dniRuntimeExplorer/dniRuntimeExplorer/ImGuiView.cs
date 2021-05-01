using System;

namespace ImGuiNET
{
    public class ImGuiView
    {
        public delegate void Callback();

        #region PopupView
        public static void PopupContextItemView(Callback callback)
        {
            if (ImGui.BeginPopupContextItem())
            {
                callback?.Invoke();
                ImGui.EndPopup();
            }
        }

        public static void PopupView(string label,Callback callback)
        {
            if (ImGui.BeginPopup(label))
            {
                callback?.Invoke();
                ImGui.EndPopup();
            }
        }
        #endregion

        #region TableView
        public static void TableSetupHeaders()
        {
            ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
            ImGui.TableHeadersRow();
        }

        public static void TableSetupHeaders(params string[] strs)
        {
            ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
            foreach (string str in strs)
            {
                ImGui.TableSetupColumn(str);
            }
            ImGui.TableHeadersRow();
        }

        public static void TableTextRow(int beginColumn, params string[] strs)
        {
            for(int i = 0; i < strs.Length; ++i)
            {
                ImGui.TableSetColumnIndex(beginColumn + i);
                ImGui.Text(strs[i]);
            }
        }

        public static void TableSetupColumn(params string[] strs)
        {
            foreach (string str in strs)
            {
                ImGui.TableSetupColumn(str);
            }
        }
        
        public static void TableColumns(params Callback[] callbacks)
        {
            int len = callbacks.Length;
            for(int i=0; i < len; ++i)
            {
                ImGui.TableNextColumn();
                callbacks[i]?.Invoke();
            }
        }

        public static void TableView(string label, Callback callback, params string[] headers)
        {
            TableView(label, callback, ImGuiTableFlags.None, headers);
        }

        public static void TableView(string label, Callback callback, int column, ImGuiTableFlags flags = ImGuiTableFlags.None)
        {
            if (ImGui.BeginTable(label, column, flags))
            {
                callback?.Invoke();
                ImGui.EndTable();
            }
        }

        public static void TableView(string label, Callback callback, ImGuiTableFlags flags, params string[] headers)
        {
            int len = headers.Length;
            if (ImGui.BeginTable(label, len, flags))
            {
                ImGuiView.TableSetupHeaders(headers);

                callback?.Invoke();
                ImGui.EndTable();
            }
        }
        #endregion

        #region ComboView
        public static void ComboView(string str_id, Callback callback,string preview_value)
        {
            if (ImGui.BeginCombo(str_id, preview_value))
            {
                callback?.Invoke();
                ImGui.EndCombo();
            }
        }

        public static void ComboView(string str_id, Callback callback, string preview_value, ImGuiComboFlags flags)
        {
            if (ImGui.BeginCombo(str_id, preview_value, flags))
            {
                callback?.Invoke();
                ImGui.EndCombo();
            }
        }
        #endregion
    }
}
