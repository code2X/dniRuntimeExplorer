using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ImGuiNET;

namespace dniRumtimeExplorer.Loader.Bepinex
{
    [BepInPlugin(__GUID__, __NAME__, "0.1")]
    class ExplorerUnityLoader : BaseUnityPlugin
    {
        public const string __NAME__ = "dniRuntimeExplorer";
        public const string __GUID__ = "0x." + __NAME__;

        RuntimeExplorerApp m_Explorer;

#if CONSOLE_EXE
    static void Main(string[] args)
    {
        RuntimeExplorerApp explorer = RuntimeExplorerApp.Instance;

        //Auto Add Untiy Assembly 
        List<string> unityAssemblyList = SeachUnityAssembly(".");
        foreach (string assemblyPath in unityAssemblyList)
        {
            explorer.LoadAssembly(assemblyPath);
        }

        explorer.LoadAssembly("ImGui.NET.dll");
        explorer.Open();
        //explorer.Run();
        Console.ReadLine();
    }
#endif

        public static List<string> SearchUnityGameAssembly(string path)
        {
            List<string> result = new List<string>();
            string[] subDirs = Directory.GetDirectories(path);
            string dataPath = string.Empty;

            foreach (string dirPath in subDirs)
            {
                if (dirPath.EndsWith("_Data"))
                {
                    dataPath = dirPath;
                    break;
                }
            }

            if (dataPath == string.Empty)
                return result;
            dataPath = dataPath + "\\Managed\\";
            if (Directory.Exists(dataPath) == false)
                return result;

            try
            {
                string[] dataFiles = Directory.GetFiles(dataPath);
                foreach (string dataFile in dataFiles)
                {
                    string fileName = Path.GetFileName(dataFile);
                    if (fileName.StartsWith("Assembly"))
                    {
                        result.Add(dataFile);
                    }
                }

            }
            catch (Exception)
            { }

            return result;
        }

        void Start()
        {
            List<string> unityAssemblyList = new List<string>();

            Utils.Caller.Try(() =>
            {
                m_Explorer = RuntimeExplorerApp.Instance;

                //Auto add game assembly to explorer app
                unityAssemblyList = SearchUnityGameAssembly(".");
                foreach (string assemblyPath in unityAssemblyList)
                {
                    Utils.Logger.Info(name);
                    m_Explorer.LoadAssembly(assemblyPath);
                }

                m_Explorer.Run();
                
            });
        }

        void Update()
        {
            if(ImGui.IsKeyDown((int)ImGuiKey.End))
            {
                if (m_Explorer.IsOpen == false)
                    m_Explorer.Open();
                else
                    m_Explorer.Close();
                Debug.Log("Explorer State:" + m_Explorer.IsOpen);
            }

        }

    }
}



