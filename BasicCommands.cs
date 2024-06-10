using QFSW.QC;
using System.Reflection;

namespace LDGKrey.QCEnabler
{
    public static class BasicCommands
    {
        public static void AddCommands()
        {
            CommandExtensions.AddCommandsFromType(typeof(QuantumConsoleProcessor));

            //AddHelp();
            //AddCommandCount();
            //AddManualHelp();
            //AddHelpCommand();
            //AddCommandList();
            //AddClearCommand();
            //AddChangeLogLevelCommand();
        }

        static void AddCommandCount()
        {
            var method = typeof(QuantumConsoleProcessor).GetProperty(nameof(QuantumConsoleProcessor.LoadedCommandCount)).GetMethod;

            CommandExtensions.AddStaticCommand(method, "command-count", "Gets the number of loaded commands.");
        }

        static void AddHelp()
        {
            var method = typeof(QuantumConsoleProcessor).GetMethod("GetHelp", BindingFlags.Static | BindingFlags.NonPublic);

            CommandExtensions.AddStaticCommand(method, "help", "Shows a basic help guide for Quantum Console.");
        }

        static void AddManualHelp()
        {
            var method = typeof(QuantumConsoleProcessor).GetMethod("ManualHelp", BindingFlags.Static | BindingFlags.NonPublic);

            CommandExtensions.AddStaticCommand(method, new string[] { "man", "manual" });
        }

        static void AddHelpCommand()
        {
            var method = typeof(QuantumConsoleProcessor).GetMethod("GenerateCommandManual", BindingFlags.Static | BindingFlags.NonPublic);

            CommandExtensions.AddStaticCommand(method, new string[] { "help", "man", "manual" });
        }

        static void AddCommandList()
        {
            var method = typeof(QuantumConsoleProcessor).GetMethod("GenerateCommandList", BindingFlags.Static | BindingFlags.NonPublic);

            CommandExtensions.AddStaticCommand(method, "commands", "Shows a basic help guide for Quantum Console.");
        }

        static void AddClearCommand()
        {
            var method = typeof(BasicCommands).GetMethod(nameof(ClearDelegate), BindingFlags.Static | BindingFlags.NonPublic);

            CommandExtensions.AddStaticCommand(method, "clear", "Clears the console window, full log stays in your persistent game folder.");
        }

        static void ClearDelegate()
            => QuantumConsole.Instance.ClearConsole();

        static void AddChangeLogLevelCommand()
        {
            var method = typeof(BasicCommands).GetMethod(nameof(ChangeLogLevelDelegate), BindingFlags.Static | BindingFlags.NonPublic);

            CommandExtensions.AddStaticCommand(method, "SetLogLevel", "Sets the loglevel. (Optional) Can set your loglevel settings permanent.");
        }

        [Command("SetLogLevel", "Sets the loglevel. (Optional) Can set your loglevel settings permanent.", MonoTargetType.Single)]
        static void ChangeLogLevelDelegate(LoggingLevel logLevel, bool saveToSettings = false)
            => QCEnabler.instance.ChangeLogLevel(logLevel, saveToSettings);
    }
}
