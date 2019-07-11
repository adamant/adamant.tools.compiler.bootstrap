using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class GenericVisitor
    {
        private static readonly ConcurrentDictionary<MethodInfo, DispatchTable> DispatchTables = new ConcurrentDictionary<MethodInfo, DispatchTable>();

        public static TReturn Accept<TInstance, TReturn>(this TInstance element, Func<TInstance, TReturn> operation)
        {
            return (TReturn)Dispatch(operation.Target, operation.Method, element);
        }

        public static void Accept<TInstance>(this TInstance element, Action<TInstance> operation)
        {
            Dispatch(operation.Target, operation.Method, element);
        }

        public static void Accept<TInstance, T1>(this TInstance element, Action<TInstance, T1> operation, T1 arg1)
        {
            Dispatch(operation.Target, operation.Method, element, arg1);
        }

        public static void Accept<TInstance, T1, T2>(this TInstance element, Action<TInstance, T1, T2> operation, T1 arg1, T2 arg2)
        {
            Dispatch(operation.Target, operation.Method, element, arg1, arg2);
        }

        private static object Dispatch(
            object visitor,
            MethodInfo operation,
            params object[] arguments)
        {
            var dispatchTable = DispatchTables.GetOrAdd(operation, DispatchTable.Build);
            return dispatchTable.Dispatch(visitor, arguments);
        }
    }
}
