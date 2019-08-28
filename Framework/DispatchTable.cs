using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    internal class DispatchTable
    {
        private readonly MethodInfo operation;
        private readonly Dictionary<Type, VisitMethod> methods;
        private readonly MethodInfo nullMethod;

        private DispatchTable(
            MethodInfo operation,
            Dictionary<Type, VisitMethod> methods,
            MethodInfo nullMethod)
        {
            this.operation = operation;
            this.methods = methods;
            this.nullMethod = nullMethod;
        }

        public static DispatchTable Build(MethodInfo operation)
        {
            var visitorType = operation.DeclaringType;
            var visitMethodName = operation.Name + "For";
            var parameters = operation.GetParameters();
            var dispatchOnType = parameters[0].ParameterType;
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly
                                              | BindingFlags.NonPublic | BindingFlags.Public
                                              | BindingFlags.Instance | BindingFlags.Static;

            // Visit Null Method
            var visitNullMethodName = visitMethodName + "Null";
            var visitNullMethodInfo = visitorType.GetMethod(visitNullMethodName, bindingFlags);


            var allConcreteSubclasses = dispatchOnType.GetAllSubtypes()
                                        .Where(t => !t.IsAbstract)
                                        .Where(t => !t.HasCustomAttribute<VisitorNotSupportedAttribute>()).ToList();

            var allVisitMethods = visitorType
                .GetMethods(bindingFlags)
                .Where(m => m.Name == visitMethodName)
                .Select(m => new VisitMethod(m)).ToHashSet();

            // TODO check all visit methods take and return compatible types (same?)

            var methods = new Dictionary<Type, VisitMethod>();
            foreach (var subclass in allConcreteSubclasses)
            {
                var matchingMethods = allVisitMethods.Where(m => m.Visits(subclass)).ToList();
                if (!matchingMethods.Any())
                    throw new Exception($"No `{visitorType.Name}.{visitMethodName}` method can handle instances of type `{subclass.Name}` (dispatching on subtype of `{dispatchOnType.Name}`");

                var bestMethod = matchingMethods.Aggregate((m1, m2) => m1.MostSpecific(m2));
                methods.Add(subclass, bestMethod);
            }

            var usedMethods = new HashSet<VisitMethod>(methods.Values);
            if (allVisitMethods.IsProperSupersetOf(usedMethods))
                throw new Exception("Some visit methods will never be called");

            return new DispatchTable(operation, methods, visitNullMethodInfo);
        }

        [DebuggerStepThrough]
        public object Dispatch(object target, object[] arguments)
        {
            if (arguments[0] == null)
            {
                if (nullMethod != null) return nullMethod.Invoke(target, arguments.Skip(1).ToArray());

                var visitorType = operation.DeclaringType;
                var methodName = operation.Name + "ForNull";
                throw new Exception(
                    $"Can't visit `null` because there is no `{visitorType.Name}.{methodName}` method.");
            }

            var dispatchType = arguments[0].GetType();
            if (!methods.TryGetValue(dispatchType, out var method))
            {
                // It wasn't directly found, that means it is a subtype
                method = methods.Single(pair => pair.Key.IsAssignableFrom(dispatchType)).Value;
                // Add so future dispatches are efficient
                methods.Add(dispatchType, method);
            }

            return method.Info.Invoke(target, arguments);
        }
    }
}
