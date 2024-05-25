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
            AddTestCommand();
            AddHelp();
            AddCommandCount();
            AddManualHelp();
            AddHelpCommand();
            AddCommandList();
        }

        static void AddTestCommand()
        {
            var method = typeof(BasicCommands).GetMethod(nameof(TestCommand));

            if (!CommandExtensions.AddStaticCommand(method, new string[] { "calcAdd", "calc" }, "Adds two integers togther"))
                Debug.LogWarning("Could not add 'calcAdd' to commands.");
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

        public static int TestCommand(int a, int b)
        {
            return a + b;
        }
    }
}
