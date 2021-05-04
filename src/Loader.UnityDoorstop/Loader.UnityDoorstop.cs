using System;
using System.Collections.Generic;
using System.IO;

namespace dniRumtimeExplorer.Loader.UnityDoorstop
{
    class Loader_UntiyDoorstop
    {
        static bool isShow = false;
        static RuntimeExplorerApp m_Explorer;

        static void Main(string[] args)
        {
            if (isShow == true)
                return;

            List<string> unityAssemblyList = new List<string>();

            Utils.Caller.Try(() =>
            {
                m_Explorer = RuntimeExplorerApp.Instance;

                //Auto add game assembly to explorer app
                unityAssemblyList = SearchUnityGameAssembly(".");
                foreach (string assemblyPath in unityAssemblyList)
                {
                    m_Explorer.LoadAssembly(assemblyPath);
                }

                m_Explorer.Run();
                isShow = true;
            });

        }

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
    }
}
