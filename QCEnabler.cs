using C3.ModKit;
using HarmonyLib;
using LDGKrey.QCEnabler.Configuration;
using QFSW.QC;
using QFSW.QC.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unfoundry;
using UnityEngine;

namespace LDGKrey.QCEnabler
{
    [UnfoundryMod(GUID)]
    public class QCEnabler : UnfoundryPlugin
    {
        public const string
            MODNAME = "QCEnabler",
            AUTHOR = "ldgKrey",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "0.3";

        static Mod _mod;
        static Dictionary<string, UnityEngine.Object> bundleMainAssets;
        static LogSource log;
        public static QCEnabler instance;

        ConsoleConfig consoleConfig;

        public override void Load(Mod mod)
        {
            instance = this;

            _mod = mod;

            bundleMainAssets = typeof(Mod).GetField("assets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod) as Dictionary<string, UnityEngine.Object>;

            log = new LogSource("QCEnabler");

            consoleConfig = ConsoleConfig.CreateOrLoad();

            InstantiateConsole();
        }

        QuantumConsole console;
        QuantumKeyConfig keyConfig;
        DynamicCanvasScaler zoomController;
        RectTransform consoleRect;

        void InstantiateConsole()
        {
            log.Log("Try instantiate console");

            var prefab = GetAsset<GameObject>("Quantum Console (SRP)");

            if(prefab == null)
            {
                log.LogError("instantiating console failed.");
                return;
            }

            var instance = GameObject.Instantiate(prefab);

            if (!instance.TryGetComponent<QuantumConsole>(out console))
            {
                log.LogError("Quantum Console (component) not found on instance.");
                return;
            }

            consoleRect = typeof(QuantumConsole).GetField("_containerRect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(console) as RectTransform;

            if (!instance.TryGetComponent<Canvas>(out var canvas))
            {
                log.LogError("Canvas (component) on console instance not found");
                return;
            }

            //console over all UI
            canvas.sortingOrder = 9999;

            //get the logs before the console was initiated
            var logs = FilteredLog();

            //insert the logs into te console window
            foreach (var log in logs)
            {
                console.LogToConsole(log);
            }

            //Key configs
            consoleConfig.ToggleKey.onChanged += OnKeyConfigChanged;
            keyConfig = typeof(QuantumConsole).GetField("_keyConfig", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(console) as QuantumKeyConfig;
            if(keyConfig == null)
                log.LogWarning("cant find keyConfig per reflection");
            if (consoleConfig.ToggleKey.Get() != KeyCode.F11)
                OnKeyConfigChanged(KeyCode.F11, consoleConfig.ToggleKey.Get());

            //Commands
            BasicCommands.AddCommands();

            //UI / Zoom
            if(!console.TryGetComponent<DynamicCanvasScaler>(out zoomController)){
                log.LogWarning("cant find zoomUIController");
            }
            else
            {
                consoleConfig.ZoomLevel.onChanged += ZoomConfigChanged;
            }

            //UI / Positioning
            consoleConfig.ConsolePosition.onChanged += OnConsolePositionChange;

            //LogLevel
            if (consoleConfig.LogLevel.Get() != LoggingLevel.Full)
                ChangeLogLevel(consoleConfig.LogLevel.Get());

            consoleConfig.LogLevel.onChanged += OnLogLevelSettingChanged;

            //Cursor lock/unlock
            console.OnActivate += OnActivate;
            console.OnDeactivate += OnDeactivate;
        }

        bool settingsChanged = false;

        #region LogLevel

        public void ChangeLogLevel(LoggingLevel newLogLevel, bool setSettings = true)
        {
            if (QuantumConsoleProcessor.loggingLevel == newLogLevel)
                return;

            log.Log($"change log level to {newLogLevel}");

            if (setSettings)
            {
                skipLogLevelchange = true;
                consoleConfig.LogLevel.Set(newLogLevel);
            }

            QuantumConsoleProcessor.loggingLevel = newLogLevel;
        }

        bool skipLogLevelchange = false;
        void OnLogLevelSettingChanged(LoggingLevel oldValue, LoggingLevel newValue)
        {
            if (skipLogLevelchange)
            {
                skipLogLevelchange = false;
                return;
            }

            ChangeLogLevel(newValue);
        }

        #endregion

        #region Zoom
        const int zoomLevelMin = 10;
        const int zoomLevelMax = 200;
        bool zoomLevelNotificationSkip = false;
        void ZoomConfigChanged(int oldValue, int newValue)
        {
            if (zoomLevelNotificationSkip)
            {
                zoomLevelNotificationSkip = false;
                return;
            }

            if (!consoleConfig.RememberZoomLevel.Get())
                return;

            if (newValue < zoomLevelMin)
            {
                consoleConfig.ZoomLevel.Set(zoomLevelMin);
                return;
            }

            if (newValue > zoomLevelMax)
            {
                consoleConfig.ZoomLevel.Set(zoomLevelMax);
                return;
            }

            zoomController.ZoomMagnification = (float)newValue / 100;
        }

        void OnZoomButtonPressed()
        {
            zoomLevelNotificationSkip = true;

            settingsChanged = true;
            consoleConfig.ZoomLevel.Set((int)(zoomController.ZoomMagnification * 100));
        }

        #endregion

        #region Position Not working currently
        bool skipOnConsolePositionChange = false;
        //have to look into the dynamicscaler because something with offsets is changed and the position cannot be reliable saved or loaded
        void OnConsolePositionChange(Vector2 oldValue, Vector2 newValue)
        {
            //if (skipOnConsolePositionChange)
            //{
            //    skipOnConsolePositionChange = false;
            //    return;
            //}

            //if(!consoleConfig.RememberPosition.Get())
            //    return;

            //log.Log($"new position {newValue}");

            //consoleRect.anchoredPosition = newValue / zoomController.ZoomMagnification;
        }

        void SavePosition()
        {
            //if (!consoleConfig.RememberPosition.Get())
            //    return;

            //var currentPosition = consoleRect.anchoredPosition * zoomController.ZoomMagnification;
            //if (currentPosition == Vector2.zero)
            //    return;

            //log.Log($"current position {currentPosition}");

            //skipOnConsolePositionChange = true;
            //consoleConfig.ConsolePosition.Set(currentPosition);
            //settingsChanged = true;
        }

        #endregion

        #region StateChange
        private void OnDeactivate()
        {
            if (GlobalStateManager.getCurrentGameState() == GlobalStateManager.GameState.Game)
            {
                GlobalStateManager.removeCursorRequirement();
            }

            SavePosition();

            if (settingsChanged)
            {
                log.Log("Save changed settings");

                consoleConfig.Save();
                settingsChanged = false;
            }
        }

        private void OnActivate()
        {
            if (GlobalStateManager.getCurrentGameState() == GlobalStateManager.GameState.Game)
            {
                GlobalStateManager.addCursorRequirement();
            }

            if (consoleConfig.ZoomLevel.Get() != 100)
                ZoomConfigChanged(100, consoleConfig.ZoomLevel.Get());

            if (consoleConfig.ConsolePosition.Get() != Vector2.zero)
                OnConsolePositionChange(Vector2.zero, consoleConfig.ConsolePosition.Get());
        }
        #endregion

        #region Keys
        void OnKeyConfigChanged(KeyCode oldKey, KeyCode newKey)
        {
            if (keyConfig == null)
                return;

            log.Log($"toggle key changed to {newKey}");

            keyConfig.ToggleConsoleVisibilityKey = new ModifierKeyCombo { Key = newKey };
        }
        #endregion

        #region Helper
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

            //unity writes while this code reads. The file is not locked.
            //https://stackoverflow.com/questions/9759697/reading-a-file-used-by-another-process its basically the same case.
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("Loading Mods from:"))
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
                log.LogError($"Missing asset: {name}");
                return null;
            }

            return (T)asset;
        }
        #endregion

        [HarmonyPatch]
        static class Patch
        {
            [HarmonyPatch(typeof(QuantumConsole), "Initialize")]
            [HarmonyPrefix]
            public static void InitializeMscorelibFix()
            {
                //Skip command table generation because of problems with mscorlib and the way QC detects scans the assemblies
                var property = typeof(QuantumConsoleProcessor).GetProperty("TableGenerated", BindingFlags.Static | BindingFlags.Public);
                property.SetValue(null, true);
            }

            [HarmonyPatch(typeof(ZoomUIController), "ZoomUp")]
            [HarmonyPostfix]
            public static void OnZoomUpPostFix(ZoomUIController __instance)
            {
                instance.OnZoomButtonPressed();
            }

            [HarmonyPatch(typeof(ZoomUIController), "ZoomDown")]
            [HarmonyPostfix]
            public static void OnZoomDownPostFix(ZoomUIController __instance)
            {
                instance.OnZoomButtonPressed();
            }
        }
    }
}
