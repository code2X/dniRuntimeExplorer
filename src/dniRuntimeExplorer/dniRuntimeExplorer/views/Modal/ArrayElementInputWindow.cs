using ImGuiNET;
using System.Reflection;
using System;
using dniRumtimeExplorer.Reflection;

namespace dniRumtimeExplorer
{
    public class ArrayElementInputWindow : ValueInputWindowBase
    {
        public override string GetPopupName() => "Array Element Value";

        //method
        Array arrayObj;
        object elementObj;
        int elementIndex;

        private ArrayElementInputWindow() { Reset(); }
        static ArrayElementInputWindow instance = new ArrayElementInputWindow();
        public static ArrayElementInputWindow GetInstance() => instance;

        public new void Reset()
        {
            base.Reset();
            this.elementObj = null;
            this.elementIndex = 0;
        }

        public void Show(Array array, object elementObj, int elementIndex)
        {
            Reset();
            this.arrayObj = array;
            this.elementObj = elementObj;
            this.elementIndex = elementIndex;
            Utils.Caller.Try(() =>
            {
                this.m_InputText = elementObj.ToString();
            });
            ShowWindow();
        }

        public override void DrawPopupContent()
        {
            DrawTableWithSingleRow("ArrayElementInputTable", elementObj.GetType(), elementIndex.ToString(), m_Errored);

            if (ImGui.Button("OK"))
            {
                bool res = ArrayElementSetter.TrySetValue(arrayObj, elementObj.GetType(), elementIndex, m_InputText);
                if (res)
                {
                    m_Errored = false;
                    CloseWindow();
                }
                else
                {
                    m_Errored = true;
                }
            }
        }
    }
}
