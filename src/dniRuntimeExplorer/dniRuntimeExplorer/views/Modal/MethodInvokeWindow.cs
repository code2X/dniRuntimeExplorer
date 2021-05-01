using ImGuiNET;
using System.Reflection;
using System;
using dniRumtimeExplorer.Reflection;

namespace dniRumtimeExplorer
{
    public class MethodInvokeWindow : IParamInputModalView
    {
        public override string GetPopupName() => "Method Invoke";

        //method
        ParameterInfo[] methodParameters;
        object methodParentObj;
        MethodInfo methodInfo;

        //error
        bool m_InvokeErrored = false;
        object m_InvokeResult = null;
        int m_ErrorRow = -1;

        string[] inputText;

        public MethodInvokeWindow() { Reset(); }

        void Reset()
        {
            this.methodInfo = null;
            this.methodParameters = null;
            this.methodParentObj = null;
            this.inputText = null;

            this.m_InvokeErrored = false;
            this.m_ErrorRow = -1;
            m_InvokeResult = null;
        }

        void ResetInputText(ParameterInfo[] parameters)
        {
            inputText = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                inputText[i] = "";
            }
        }

        public void Show(MethodInfo method, ParameterInfo[] parameters, object parentObj = null)
        {
            Reset();
            ResetInputText(parameters);
            this.methodInfo = method;
            this.methodParameters = parameters;
            this.methodParentObj = parentObj;
            ShowWindow();

        }

        public override void DrawPopupContent()
        {
            DrawTable();

            if (ImGui.Button("Call"))
            {
                CallMethod();
            }

            if(m_InvokeResult != null)
            {
                ImGui.SameLine();
                ImGui.Text("Result: " + m_InvokeResult);
            }
        }

        void DrawTable()
        {
            ImGuiView.TableView("MethodInvokeTable", () =>
            {
                for (int i = 0; i < inputText.Length; ++i)
                {
                    ImGui.TableNextRow();
                    if (m_ErrorRow == i)
                        paramTable.DrawRow(methodParameters[i].ParameterType, methodParameters[i].Name, ref inputText[i], true);
                    else
                        paramTable.DrawRow(methodParameters[i].ParameterType, methodParameters[i].Name, ref inputText[i]);
                }
            }, "Type", "Name", "Value", "Error");
        }


        void CallMethod()
        {
            MethodInvoker invoke = new MethodInvoker(methodInfo, methodParentObj);
            int res = invoke.Invoke(out m_InvokeResult, inputText);
            if (res == 0 && m_InvokeResult != null)
            {
                InvokeSuccess(m_InvokeResult);
            }
            else if (res == -1)
            {
                InvokeError();
            }
            else
            {
                InputError(res - 1);
            }
        }

        void InvokeSuccess(object outObj)
        {
            m_ErrorRow = -1;

            Type resultType = outObj.GetType();
            if (CsharpKeywords.GeneralTypes.Contains(resultType) == false
                && resultType.IsEnum == false)
            {
                RuntimeExplorerApp.Instance.ClassWindow.AddNewInstance(methodInfo.Name, typeof(object), outObj);
                CloseWindow();
            }             
        }

        void InvokeError()
        {
            m_InvokeErrored = true;

            ImGui.SameLine();
            ImGui.Text("Invoke Error");
        }

        void InputError(int line)
        {
            m_ErrorRow = line;
        }
    }
}
