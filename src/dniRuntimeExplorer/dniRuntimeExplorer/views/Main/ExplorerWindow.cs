using ImGuiNET;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using dniRumtimeExplorer.Utils;
using dniRumtimeExplorer.Reflection;

namespace dniRumtimeExplorer.Window
{
    class CategoryClass
    {
        public SortedDictionary<string, SortedDictionary<string, Type>> main;
        public SortedDictionary<string, SortedDictionary<string, Type>> auto;

        public void AutoCluster(SortedDictionary<string, Type> name2type)
        {
            main = ClassCluster.MainCluster(name2type);
            auto = ClassCluster.AutoCluster(name2type);
        }

        public void AutoCluster(Assembler assembler)
        {
            SortedDictionary<string, Type> name2type = new SortedDictionary<string, Type>();
            foreach(Type type in assembler.AssemblyTypes)
            {
                name2type.Add(type.FullName, type);
            }
            AutoCluster(name2type);
        }
    }

    public class ExplorerWindow : ImGuiEx.Window
    {
        class AssemblyModel
        {
            public Assembler dll = null;
            public CategoryClass category = null;
        }
   
        public override string WindowName => "Assembly Explorer";
        public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.MenuBar;

        //string: Assembler Location 
        Dictionary<string, AssemblyModel> m_Assemblers = new Dictionary<string, AssemblyModel>();
        string m_ProcessName = "";
        string m_SearchText = "";

        //OpenFileDialog m_FileDialog = new OpenFileDialog();

        #region Event
        public enum EClassEvent
        {
            LClicked,
            DoubleLClicked,
            RClicked,
            Hovered,
        }

        public delegate void ClassAction(EClassEvent eClassEvent, Type classType);
        public event ClassAction OnClassEvent;
        #endregion

        public ExplorerWindow()
        {
            //m_FileDialog.Filter = "Assembly File(*.dll)|*.dll;";
            //m_FileDialog.ValidateNames = true;
            //m_FileDialog.CheckPathExists = true;
            //m_FileDialog.CheckFileExists = true;

            m_ProcessName = Process.GetCurrentProcess().MainModule.ModuleName;
        }

        /// <summary>
        /// Add assembler to explorer window display
        /// </summary>
        public void AddAssembler(Assembler assembler)
        {
            if (assembler == null)
                return;

            string location = assembler.Location;
            if (m_Assemblers.ContainsKey(location))
                return;

            //Add assembler model
            AssemblyModel assemblyModel = new AssemblyModel();
            assemblyModel.dll = assembler;

            //Add classified assembler class
            CategoryClass clustering = new CategoryClass();
            clustering.AutoCluster(assembler);
            assemblyModel.category = clustering;

            m_Assemblers.Add(location, assemblyModel);

            Console.WriteLine(assembler.Assembly.Location);
        }

        /// <summary>
        /// Draw Window's Content
        /// </summary>
        protected override void DrawWindowContent()
        {
            //Menu
            if(ImGui.BeginMenuBar())
            {
                DrawFileMenu();
                ImGui.EndMenuBar();
            }

            ImGui.Text("Proccess:" + m_ProcessName);
            ImGui.InputTextWithHint("", "type to search", ref m_SearchText, 20);

            DrawAssemblersHeader();
        }

        void DrawFileMenu()
        {
            if (ImGui.BeginMenu("File"))
            {
                if(ImGui.MenuItem("Open"))
                {
                    //if (m_FileDialog.ShowDialog() == DialogResult.OK)
                    //{
                    //    RuntimeExplorerApp.Instance.LoadAssembly(m_FileDialog.FileName);
                    //}
                }
                ImGui.EndMenu();
            }
        }

        /// <summary>
        /// 1. Draw Assembly Collapsing Header 
        /// </summary>
        protected virtual void DrawAssemblersHeader()
        {
            foreach (var assembly in m_Assemblers)
            {
                string lable = assembly.Value.dll.Name + "##" + assembly.Value.dll.Location;

                if (ImGui.CollapsingHeader(lable))
                {
                    DrawCategoryHeader(assembly.Value, lable);
                }
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(assembly.Key);
                }
                ImGui.Separator();
            }
        }

        /// <summary>
        /// 2. Draw Classified Assembly Classes Collapsing Header 
        /// </summary>
        void DrawCategoryHeader(AssemblyModel model,string label)
        {
            ImGui.Text("Main Class");
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Assembly main class category");
            }
            foreach (var name2type in model.category.main)
            {
                ImGui.Text("  ");
                ImGui.SameLine();

                if (ImGui.CollapsingHeader(name2type.Key + "##" + label))
                {
                    DrawClassTable(name2type.Value, "##main" + label);
                }
            }

            ImGui.Text("Auto Category");
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Automatic classification assembly class");
            }
            foreach (var name2type in model.category.auto)
            {
                ImGui.Text("  ");
                ImGui.SameLine();

                if (ImGui.CollapsingHeader(name2type.Key + "##" + label))
                {
                    DrawClassTable(name2type.Value, "##auto" + label);
                }
            }
        }

        /// <summary>
        /// 3. Draw Class Table
        /// </summary>
        static ImGuiTableFlags tableFlags = 
        ImGuiTableFlags.SizingFixedFit | 
            ImGuiTableFlags.Resizable | 
            ImGuiTableFlags.Reorderable | 
            ImGuiTableFlags.Hideable;
        protected virtual void DrawClassTable(SortedDictionary<string, Type> classDict, string label)
        {
            ImGui.Text("    ");
            ImGui.SameLine();
            ImGuiView.TableView("Tabel" + label, () =>
            {
                foreach (var class2type in classDict)
                {

                    if (class2type.Key.IndexOf(m_SearchText) != -1)
                    {
                        ImGui.TableNextRow();
                        DrawClassTableRow(class2type.Value, label);
                    }
                }
            }, tableFlags, "Class", "Base Class");
        }

        /// <summary>
        /// 4. Draw Class Table Row
        /// </summary>
        protected virtual void DrawClassTableRow(Type classType, string label)
        {
            label = classType.ToString();

            //避免导致异常
            if (classType is null)
                return;

            //Class Type
            ImGui.TableNextColumn();
            if (ImGui.Selectable(label))
            {
                OnClassEvent?.Invoke(EClassEvent.LClicked, classType);
            }

            //避免导致异常
            if (classType.BaseType is null)
                return;

            //Class Base Type
            ImGui.TableNextColumn();
            if (ImGui.Selectable(classType.BaseType.Name))
            {
                OnClassEvent?.Invoke(EClassEvent.LClicked, classType.BaseType);
            }
        }

    }
}
