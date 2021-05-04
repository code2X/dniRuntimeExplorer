using System;
using dniRumtimeExplorer.Window;
using dniRumtimeExplorer.Reflection;
using System.Collections.Generic;
using ImGuiNET;

namespace dniRumtimeExplorer
{
    public class RuntimeExplorerApp : ImGuiEx.Application
    {
        bool m_bShow = true;

        static RuntimeExplorerApp __instance = new RuntimeExplorerApp();
        public static RuntimeExplorerApp Instance
        {
            get => __instance;
            set => __instance = value;
        }

        //Main Windows
        ExplorerWindow m_ExplorerWindow = new ExplorerWindow();
        ClassWindow m_ClassWindow = new ClassWindow();

        //Intput Modal Windows
        FieldValueInputWindow m_FieldValueInputWindow = new FieldValueInputWindow();
        PropertyValueInputWindow m_PropertyValueInputWindow = new PropertyValueInputWindow();
        MethodInvokeWindow m_MethodInvokeWindow = new MethodInvokeWindow();

        public ExplorerWindow ExplorerWindow => m_ExplorerWindow;
        public ClassWindow ClassWindow => m_ClassWindow;

        public FieldValueInputWindow FieldValueInputWindow => m_FieldValueInputWindow;
        public PropertyValueInputWindow PropertyValueInputWindow => m_PropertyValueInputWindow;
        public MethodInvokeWindow MethodInvokeWindow => m_MethodInvokeWindow;

        RuntimeExplorerApp()
        {
            m_ExplorerWindow.OnClassEvent += OnExplorerViewClassEvent;
        }

        private void OnExplorerViewClassEvent(ExplorerWindow.EClassEvent eClassEvent, Type classType)
        {
            switch (eClassEvent)
            {
                case ExplorerWindow.EClassEvent.LClicked:
                    m_ClassWindow.OpenTab(classType);
                    break;
            }
        }

        public bool IsOpen => m_bShow;
        public void Open() => m_bShow = true;
        public void Close() => m_bShow = false;

        protected override void OnGui()
        {
            if (ImGui.IsKeyDown((int)ImGuiKey.End))
            {
                if (IsOpen == false)
                    Open();
                else
                    Close();
            }

            if (!m_bShow) return;

            Utils.Caller.Try(() =>
            {
                ExplorerWindow.OnGui();
                ClassWindow.OnGui();

                FieldValueInputWindow.OnGui();
                PropertyValueInputWindow.OnGui();
                MethodInvokeWindow.OnGui();

                ArrayInfoWindow.GetInstance().OnGui();
                ArrayElementInputWindow.GetInstance().OnGui();

                foreach (Callback callback in m_GuiCallbacks)
                {
                    callback();
                }
            });
        }

        List<Callback> m_GuiCallbacks = new List<Callback>();
        public void AddGuiFunction(Callback callback)
        {
            m_GuiCallbacks.Add(callback);
        }

        public bool LoadAssembly(string path)
        {
            Assembler assembler = new Assembler();
            bool res = assembler.Load(path);
            if (res)
            {
                m_ExplorerWindow.AddAssembler(assembler);
            }
            return res;
        }
    }
}

