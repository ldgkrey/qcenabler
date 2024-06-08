using QFSW.QC;
using Unfoundry;
using UnityEngine;

namespace LDGKrey.QCEnabler.Configuration
{
    public class ConsoleConfig
    {
        Config config;

        //Position
        public TypedConfigEntry<Vector2> ConsolePosition;
        public TypedConfigEntry<bool> RememberPosition;

        //Keys
        public TypedConfigEntry<KeyCode> ToggleKey;

        //Zoom
        public TypedConfigEntry<int> ZoomLevel;
        public TypedConfigEntry<bool> RememberZoomLevel;

        //LogLevel
        public TypedConfigEntry<LoggingLevel> LogLevel;

        public static ConsoleConfig CreateOrLoad()
        {
            var configuration = new ConsoleConfig();

            configuration.config = new Config(QCEnabler.GUID)
                .Group("Position")
                    .Entry<bool>(out configuration.RememberPosition, "(Disabled)Remember closing position", false, false, "If enabled the console position will be remebered after closing.(Disabled in code, currently not working)")
                    .Entry<Vector2>(out configuration.ConsolePosition, "(Disabled)Console position on screen", Vector2.zero , false, "Set the position of the console on screen.(Disabled in code, currently not working)")
                .EndGroup()
                .Group("Position")
                    .Entry<bool>(out configuration.RememberZoomLevel, "Remember zoom level", false, false, "If enabled the console zoom level will be remebered after closing.")
                    .Entry<int>(out configuration.ZoomLevel, "Console zoom level", 100, false, "Set the zoom level of the console.")
                .EndGroup()
                .Group("KeyConfig")
                    .Entry<KeyCode>(out configuration.ToggleKey, "Open/Close Key", KeyCode.F11, false, "Define your show/hide key. Use the Unity KeyCode reference to saw what values are possible.")
                .EndGroup()
                .Group("Log")
                    .Entry<LoggingLevel>(out configuration.LogLevel, "LogLevel", LoggingLevel.Full, false, "Set the log level of messages shown in the console.")
                .EndGroup()
                .Load()
                .Save();

            return configuration;
        }

        public void Save()
        {
            config.Save();
        }
    }
}
