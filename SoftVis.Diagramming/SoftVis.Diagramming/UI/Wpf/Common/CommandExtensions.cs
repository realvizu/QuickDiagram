using System;
using System.Windows.Input;

namespace Codartis.SoftVis.UI.Wpf.Common
{
    /// <summary>
    /// Extension methods to conveniently execute ICommands with typed parameters or no parameter.
    /// </summary>
    public static class CommandExtensions
    {
        public static void Execute(this ICommand command) => command.Execute(null);

        public static void Execute<T>(this ICommand command, T parameter) => command.Execute(parameter);

        public static void Execute<T1,T2>(this ICommand command, T1 param1, T2 param2) 
            => command.Execute(Tuple.Create(param1, param2));

        public static void Execute<T1, T2, T3>(this ICommand command, T1 param1, T2 param2, T3 param3)
            => command.Execute(Tuple.Create(param1, param2, param3));
    }
}
