using ImGuiNET;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using dniRumtimeExplorer.Utils;
using dniRumtimeExplorer.Reflection;

namespace dniRumtimeExplorer.Window
{
    using ClassSubCategory = SortedDictionary<string, Type>;
    using ClassCategory = SortedDictionary<string, SortedDictionary<string, Type>>;

    class CategoryClass
    {
        public ClassCategory main = new ClassCategory();
        public ClassCategory auto = new ClassCategory();
        public ClassCategory namespaces = new ClassCategory();

        public void AutoCluster(ClassSubCategory name2type)
        {
            main = ClassCluster.MainClass(name2type);
            auto = ClassCluster.ClassCategory(name2type);
            namespaces = ClassCluster.NamespaceClasses(name2type);
        }

        public void AutoCluster(Assembler assembler)
        {
            ClassSubCategory name2type = new ClassSubCategory();
            foreach(Type type in assembler.AssemblyTypes)
            {
                name2type.Add(type.FullName, type);
            }
            AutoCluster(name2type);
        }
    }

    public class ExplorerWindow : ImGuiEx.Window
    {
        public override string WindowName => "Assembly Explorer";
        public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.MenuBar;

        static ImGuiTableFlags s_TableFlags =
    ImGuiTableFlags.SizingFixedFit |
    ImGuiTableFlags.Resizable |
    ImGuiTableFlags.Reorderable |
    ImGuiTableFlags.Hideable;

        class AssemblyModel
        {
            public Assembler dll = null;
            public CategoryClass category = null;
        }
  

        //Key: Assembler Location 
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
            ImGui.Text("FPS:" + (1.0f / ImGui.GetIO().DeltaTime).ToString());
            ImGui.Text("Proccess:" + m_ProcessName);
            ImGui.InputTextWithHint("", "type to search", ref m_SearchText, 20);

            DrawAssemblersHeader();
        }

        void DrawFileMenu()
        {
            /*
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
            */
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
                    DrawCategoryHeaders(assembly.Value, lable);
                }
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(assembly.Key);
                }
                ImGui.Separator();
            }
        }

        /// <summary>
        /// 2. Draw Classified Assembly Classes Collapsing Headers
        /// </summary>
        void DrawCategoryHeaders(AssemblyModel model,string label)
        {
            //Main class
            DrawCategoryHeader(model.category.main, label, "Main Classes", "Assembly main class category");

            //Namespace
            DrawCategoryHeader(model.category.namespaces, label, "Namespaces", "Namespaces classes");

            //Auto Category
            DrawCategoryHeader(model.category.auto, label, "Class Category", "classes automatic classification by name");

        }

        /// <summary>
        /// 3. Draw Single Classified Assembly Classes Collapsing Header
        /// </summary>
        void DrawCategoryHeader(
            ClassCategory category, 
            string label, 
            string title,
            string tooltip = "")
        {
            ImGui.Text(title);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(tooltip);

            foreach (var name2type in category)
            {
                PadLeft("  ", () =>
                {
                    if (ImGui.CollapsingHeader(name2type.Key + "##Header" + label))
                        DrawClassTable(name2type.Value, "##tabel" + label);
                });
            }
        }

        void PadLeft(string pad, Callback callback)
        {
            ImGui.Text(pad);
            ImGui.SameLine();
            callback();
        }

        /// <summary>
        /// 4. Draw Class Table
        /// </summary>
        protected virtual void DrawClassTable(ClassSubCategory classDict, string label)
        {
            PadLeft("    ", () =>
             {
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
                 }, s_TableFlags, "Class Name","Class Type", "Base Class");

             });
        }

        /// <summary>
        /// 5. Draw Class Table Row
        /// </summary>
        protected virtual void DrawClassTableRow(Type classType, string label)
        {
            label = classType.ToString();

            //Avoid framework exception
            if (classType is null)
                return;

            //Class Type
            ImGui.TableNextColumn();
            ImGui.Text(classType.Name);

            //Class Type
            ImGui.TableNextColumn();
            if (ImGui.Selectable(label))
            {
                OnClassEvent?.Invoke(EClassEvent.LClicked, classType);
            }

            //Avoid framework exception
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
