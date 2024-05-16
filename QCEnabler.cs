using C3.ModKit;
using HarmonyLib;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace QCEnabler
{
    [HarmonyPatch]
    public class QCEnabler
    {
        const string identifier = "ldgKrey_QCEnabler";

        private static Dictionary<string, UnityEngine.Object> bundleMainAssets;

        [HarmonyPatch(typeof(MainMenuManager), "Start")]
        [HarmonyPostfix]
        public static void MainMenuManagerStart(MainMenuManager __instance)
        {
            //Debug.Log("QCEnabler Start");
            //Debug.Log("Try to get Mod AssetBundle");
            var mod = ModManager.getAllMods().FirstOrDefault(x => x.modInfo.identifier == identifier);

            if (mod == null)
            {
                Debug.LogError("Cant found mod?");
                return;
            }

            bundleMainAssets = typeof(Mod).GetField("assets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod) as Dictionary<string, UnityEngine.Object>;

            //foreach (var key in bundleMainAssets.Keys)
            //{
            //    Debug.Log($"asset found: {key}");
            //}

            var prefab = GetAsset<GameObject>("Quantum Console (SRP)");

            var instance = GameObject.Instantiate(prefab);
            if (!instance.TryGetComponent<QuantumConsole>(out var console))
            {
                Debug.LogError("Quantum Console not found on prefab instance");
                return;
            }

            if (!instance.TryGetComponent<Canvas>(out var canvas))
            {
                Debug.LogError("Canvas on console object not found");
                return;
            }

            //console over all UI
            canvas.sortingOrder = 9999;

            //get the logs before the console was initiated
            var logs = FilteredLog();
            //Debug.Log($"length of logs array {logs.Count}");

            //insert the logs into te console window
            foreach (var log in logs)
            {
                console.LogToConsole(log);
            }

            //console.Activate(true);
        }

        [HarmonyPatch(typeof(QuantumConsole), "Initialize")]
        [HarmonyPrefix]
        public static void InitializeMscorelibFix()
        {
            //Skip command table generation
            var property = typeof(QuantumConsoleProcessor).GetProperty("TableGenerated", BindingFlags.Static | BindingFlags.Public);
            property.SetValue(null, true);
        }

        public static string[] Filter = new string[]
        {
            "Can't find custom attr",
            "Fallback handler",
        };
        public static List<string> FilteredLog()
        {
            var path = Path.Combine(Application.persistentDataPath, "Player.log");
            var logMessages = new List<string>();
            bool enableLogging = false;

            //unity writes while we read but the file is not locked.
            //https://stackoverflow.com/questions/9759697/reading-a-file-used-by-another-process its basically the same case.
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                string line;

                while ((line = sr.ReadLine()) != null) 
                {
                    if(line.StartsWith("Loading Mods from:"))
                    {
                        enableLogging = true;
                    }

                    if (!enableLogging)
                        continue;

                    if (Filter.Any(x => line.StartsWith(x)))
                        continue;

                    logMessages.Add(line);
                }
            }

            return logMessages;
        }

        public static T GetAsset<T>(string name) where T : UnityEngine.Object
        {
            if (!bundleMainAssets.TryGetValue(name, out var asset))
            {
                Debug.Log($"Missing asset '{name}'");
                return null;
            }

            return (T)asset;
        }
    }
}
