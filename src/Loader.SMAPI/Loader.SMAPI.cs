using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace dniRumtimeExplorer.Loader.SMAPI
{
    class Loader_SMAPI : Mod
    {
        static RuntimeExplorerApp m_Explorer;

        public override void Entry(IModHelper helper)
        {
            List<string> unityAssemblyList = new List<string>();

            Console.WriteLine("Loader.SMAPI");

            Utils.Caller.Try(() =>
            {
                m_Explorer = RuntimeExplorerApp.Instance;

                //Auto add game assembly to explorer app
                unityAssemblyList = SearchGameAssembly(".");
                foreach (string assemblyPath in unityAssemblyList)
                {
                    m_Explorer.LoadAssembly(assemblyPath);
                }

                m_Explorer.Run();
            });
        }

        public static List<string> SearchGameAssembly(string path)
        {
            List<string> result = new List<string>();
            result.Add("Stardew Valley.exe");
            result.Add("StardewValley.GameData.dll");

            result.Add("Netcode.dll");
            result.Add("GalaxyCSharpGlue.dll");
            result.Add("GalaxyCSharp.dll");
            result.Add("Galaxy.dll");

            return result;
        }
    }
}
