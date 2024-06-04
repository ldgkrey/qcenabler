using QFSW.QC;
using System;
using System.Reflection;
using UnityEngine;

namespace LDGKrey.QCEnabler
{
    public static class BasicCommands
    {
        public static void AddCommands()
        {
            AddHelp();
            AddCommandCount();
            AddManualHelp();
            AddHelpCommand();
            AddCommandList();
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
    }
}
