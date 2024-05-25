using QFSW.QC;
using System;
using System.Reflection;
using UnityEngine;

namespace LDGKrey.QCEnabler
{
    public static class CommandExtensions
    {
        public static bool AddStaticCommand(MethodInfo method, string alias, string description = "")
            => AddStaticCommandLogic(method, alias, description);

        public static bool AddStaticCommand(MethodInfo method, string[] aliases, string description = "")
        {
            var result = true;
            foreach (var alias in aliases)
            {
                if (!AddStaticCommandLogic(method, alias, description))
                    result = false;
            }

            return result;
        }

        static bool AddStaticCommandLogic(MethodInfo method, string alias, string description = "")
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
