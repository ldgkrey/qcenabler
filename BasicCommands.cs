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
            try
            {
                CommandAttribute commandAttribute = new CommandAttribute("command-count", "Gets the number of loaded commands.");

                CommandData commandData = new CommandData(typeof(QuantumConsoleProcessor).GetProperty(nameof(QuantumConsoleProcessor.LoadedCommandCount)).GetMethod, commandAttribute);

                QuantumConsoleProcessor.TryAddCommand(commandData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }
        }

        static void AddHelp()
        {
            try
            {
                CommandAttribute commandAttribute = new CommandAttribute("help", "Shows a basic help guide for Quantum Console.");

                CommandData commandData = new CommandData(typeof(QuantumConsoleProcessor).GetMethod("GetHelp", BindingFlags.Static | BindingFlags.NonPublic), commandAttribute);

                QuantumConsoleProcessor.TryAddCommand(commandData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }
        }

        static void AddManualHelp()
        {
            try
            {
                CommandAttribute commandAttribute = new CommandAttribute("man");

                CommandData commandData = new CommandData(typeof(QuantumConsoleProcessor).GetMethod("ManualHelp", BindingFlags.Static | BindingFlags.NonPublic), commandAttribute);

                QuantumConsoleProcessor.TryAddCommand(commandData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }
        }

        static void AddHelpCommand()
        {
            try
            {
                var method = typeof(QuantumConsoleProcessor).GetMethod("GenerateCommandManual", BindingFlags.Static | BindingFlags.NonPublic);

                CommandAttribute commandAttribute = new CommandAttribute("help");
                CommandData commandData = new CommandData(method, commandAttribute);
                QuantumConsoleProcessor.TryAddCommand(commandData);

                commandAttribute = new CommandAttribute("man");
                commandData = new CommandData(method, commandAttribute);
                QuantumConsoleProcessor.TryAddCommand(commandData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }
        }

        static void AddCommandList()
        {
            try
            {
                CommandAttribute commandAttribute = new CommandAttribute("commands", "Shows a basic help guide for Quantum Console.");

                CommandData commandData = new CommandData(typeof(QuantumConsoleProcessor).GetMethod("GenerateCommandList", BindingFlags.Static | BindingFlags.NonPublic), commandAttribute);

                QuantumConsoleProcessor.TryAddCommand(commandData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }
        }

        public static int TestCommand(int a, int b)
        {
            return a + b;
        }
    }
}
