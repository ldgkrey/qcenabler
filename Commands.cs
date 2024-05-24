using QFSW.QC;
using System;
using System.Reflection;
using UnityEngine;

namespace LDGKrey.QCEnabler
{
    public static class Commands
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
            var method = typeof(Commands).GetMethod(nameof(TestCommand));

            if (!AddCommand(method, new string[] { "calcAdd", "calc" }, "Adds two integers togther"))
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

        //Utility
        public static bool AddCommand(MethodInfo method, string alias, string description = "")
            => AddCommandLogic(method, alias, description);

        public static bool AddCommand(MethodInfo method, string[] aliases, string description = "")
        {
            var result = true;
            foreach(var alias in aliases)
            {
                if (!AddCommandLogic(method, alias, description))
                    result = false;
            }

            return result;
        }

        static bool AddCommandLogic(MethodInfo method, string alias, string description = "")
        {
            try
            {
                CommandAttribute commandAttribute = new CommandAttribute(alias, description);
                CommandData commandData = new CommandData(method, commandAttribute);
                return QuantumConsoleProcessor.TryAddCommand(commandData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }
    }
}
