using dniRumtimeExplorer.ClassViews;
using dniRumtimeExplorer.Drawer;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace dniRumtimeExplorer.Window
{
    struct ClassInstance
    {
        public string name;
        public Type parent;
        public Type type;
        public object instance;

        public ClassInstance(string name,Type parent, Type type, object instance)
        {
            this.name = name;
            this.parent = parent;
            this.type = type;
            this.instance = instance;
        }
    }

    public class ClassWindow : ImGuiEx.Window
    {
        //Views
        TabBarView m_TabBarView = new TabBarView();
        List<IClassInfoView> m_ClassSubViews = new List<IClassInfoView>();
        ITypeDrawer m_TypeDrawer = new DefaultTypeDrawer();

        //Current type and Instance
        Type m_CurrentType = null;
        object m_CurrentInstance = null;
        string m_CurrentInstanceName = "";

        //Instance list
        Dictionary<int, ClassInstance> m_Instances = new Dictionary<int, ClassInstance>();
        int m_NewInstanceIndex = 0;

        public static ClassWindow Instance { get; set; } = new ClassWindow();

        public ClassWindow()
        {
            CloseWindow();
            m_TabBarView.OnTabEvent += OnTabEvent;

            m_ClassSubViews.Add(new MethodView());
            m_ClassSubViews.Add(new PropertyView());
            m_ClassSubViews.Add(new FieldView());
        }

        private void OnTabEvent(TabBarView.ETabEvent eTabEvent, Type type)
        {
            switch(eTabEvent)
            {
                case TabBarView.ETabEvent.Opened:
                    SetNewType(type);
                    break;
                case TabBarView.ETabEvent.Removed:
                    if (m_TabBarView.TabCount == 0)
                    {
                        m_CurrentType = null;
                        m_CurrentInstance = null;
                    }
                    break;
            }
        }

        public virtual void SetNewType(Type type, object instance = null)
        {
            m_CurrentType = type;
            m_CurrentInstance = instance;
            foreach (IClassInfoView view in m_ClassSubViews)
            {
                view.ShowTypeView(m_CurrentType, instance);
            }
        }

        void SetNewType(ClassInstance classInstance)
        {
            m_CurrentType = classInstance.type;
            m_CurrentInstance = classInstance.instance;
            m_CurrentInstanceName = classInstance.name;

            foreach (IClassInfoView view in m_ClassSubViews)
            {
                view.ShowTypeView(m_CurrentType, m_CurrentInstance);
            }
        }

        public virtual bool OpenTab(Type type)
        {
            ShowWindow();
            m_TabBarView.OpenTab(type);
            return true;
        }

        /// <summary>
        /// Add a new instance to instance list
        /// </summary>
        public virtual bool AddNewInstance(string name, Type parent,object instance)
        {
            if (instance == null)
                return false;
            bool res = Utils.Caller.Try(() =>
            {
                ShowWindow();
                ClassInstance classInstance = new ClassInstance(name, parent, instance.GetType(), instance);
                m_Instances.Add(m_NewInstanceIndex, classInstance);
                ++m_NewInstanceIndex;
            });

            return res;
        }

        protected override void DrawWindowContent()
        {
            m_TabBarView.OnGui();
            ImGuiView.TableView("##ClassWindowTable", () =>
             {
                 ImGui.TableNextRow();

                 ImGui.TableNextColumn();
                 DrawInstanceList();

                 ImGui.TableNextColumn();
                 DrawSubViews();
             }, 2,ImGuiTableFlags.Resizable);
        }

        /// <summary>
        /// Class info views on window right side
        /// </summary>
        void DrawSubViews()
        {
            Utils.Caller.Try(() =>
            {
                if (m_CurrentType is null == false)
                    ImGui.Text("Current Type:  " + m_CurrentType.FullName);
                if (m_CurrentInstance != null)
                    ImGui.Text("Current Instance:  " + m_CurrentInstanceName);
                foreach (IClassInfoView view in m_ClassSubViews)
                {
                    view.OnGui();
                };
            });
        }

        /// <summary>
        /// Instance list on window left side
        /// </summary>
        public void DrawInstanceList()
        {
            ImGui.Text("Instance List");
            ImGuiView.TableView("InstanceListView", () =>
            {
                foreach (var pair in m_Instances)
                {
                    ImGui.TableNextRow();
                    //Instance Type
                    ImGui.TableNextColumn();
                    m_TypeDrawer.DrawType(pair.Value.type);
                    //Instance name
                    ImGui.TableNextColumn();
                    DrawInstanceButton(pair.Key, pair.Value);
                    //Instance Parent Type
                    ImGui.TableNextColumn();
                    m_TypeDrawer.DrawType(pair.Value.parent);
                    ///Remove
                    ImGui.TableNextColumn();
                    if(ImGui.Button("X##RemoveInstanceList" + pair.Key))
                    {
                        m_Instances.Remove(pair.Key);
                        break;
                    }             
                }
            }, ImGuiTableFlags.Resizable, "Type", "Instance", "Parent", "Remove");
        }

        void DrawInstanceButton(int index,ClassInstance classInstance)
        {
            Utils.Caller.Try(() =>
            {
                if (classInstance.instance != null)
                {
                    if (ImGui.Selectable(classInstance.name + "##InstanceList" + index))
                    {
                        //SetNewType(classInstance.type, classInstance.instance);
                        SetNewType(classInstance);
                    }
                }
            });
        }

        public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar;
        public override string WindowName => "Class Window";
    }

}
