using System;
using dniRumtimeExplorer.Window;
using dniRumtimeExplorer;
using dniRumtimeExplorer.Reflection;
using dniRumtimeExplorer.Utils;

    public class RuntimeExplorerApp: ImGuiEx.Application
    {

        static RuntimeExplorerApp __instance = null;
        public static RuntimeExplorerApp Instance
        {
            get
            {
                if (__instance == null)
                    __instance = new RuntimeExplorerApp();
               return __instance;
            }
            set => __instance = value;
        }

        // Window Views
        ExplorerWindow m_ExplorerWindow = new ExplorerWindow();
        ClassWindow m_ClassWindow = new ClassWindow();

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

        bool isShow = true;
        public bool IsOpen => isShow;
        public void Open() => isShow = true;
        public void Close() => isShow = false;

        protected override void OnGui()
        {
            if (!isShow) return;
    
            Caller.Try(() =>
            {
                ExplorerWindow.OnGui();
                ClassWindow.OnGui();

                FieldValueInputWindow.OnGui();
                PropertyValueInputWindow.OnGui();
                MethodInvokeWindow.OnGui();

                ArrayInfoWindow.GetInstance().OnGui();
                ArrayElementInputWindow.GetInstance().OnGui();
            });
        }

        public bool LoadAssembly(string path)
        {
            Assembler assembler = new Assembler();
            bool res = assembler.Load(path);
            if(res)
            {
                m_ExplorerWindow.AddAssembler(assembler);
            }
            return res;
        }
    }

