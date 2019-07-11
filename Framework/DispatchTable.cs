using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    internal class DispatchTable
    {
        private readonly Dictionary<Type, MethodInfo> methods;

        private DispatchTable(Dictionary<Type, MethodInfo> methods)
        {
            this.methods = methods;
        }

        public static DispatchTable Build(MethodInfo operation)
        {
            var visitorType = operation.DeclaringType;
            var visitMethodName = operation.Name + "For";
            var parameters = operation.GetParameters();
            var dispatchOnType = parameters[0].ParameterType;

            var allConcreteSubclasses = dispatchOnType.GetAllSubtypes()
                                        .Where(t => !t.IsAbstract)
                                        .Where(t => !t.HasAttribute<VisitorNotSupportedAttribute>()).ToList();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly
                                              | BindingFlags.NonPublic | BindingFlags.Public
                                              | BindingFlags.Instance | BindingFlags.Static;
            var allVisitMethods = visitorType
                .GetMethods(bindingFlags)
                .Where(m => m.Name == visitMethodName).ToHashSet();

            // TODO check all visit methods take and return compatible types (same?)

            var methods = new Dictionary<Type, MethodInfo>();
            foreach (var subclass in allConcreteSubclasses)
            {
                var matchingMethods = allVisitMethods.Where(m =>
                    m.GetParameters()[0].ParameterType.IsAssignableFrom(subclass))
                    .ToList();
                if (!matchingMethods.Any())
                    throw new Exception($"No `{visitorType.Name}.{visitMethodName}` method can handle instances of type `{subclass.Name}` (dispatching on subtype of `{dispatchOnType.Name}`");

                var bestMethod = matchingMethods.Aggregate(MoreSpecificMethod);
                methods.Add(subclass, bestMethod);
            }

            var usedMethods = new HashSet<MethodInfo>(methods.Values);
            if (allVisitMethods.IsProperSupersetOf(usedMethods))
                throw new Exception("Some visit methods will never be called");

            return new DispatchTable(methods);
        }

        public static MethodInfo MoreSpecificMethod(MethodInfo m1, MethodInfo m2)
        {
            var p1 = m1.GetParameters()[0].ParameterType;
            var p2 = m2.GetParameters()[0].ParameterType;


            var m1MoreSpecific = p2.IsAssignableFrom(p1);
            var m2MoreSpecific = p1.IsAssignableFrom(p2);

            if (m1MoreSpecific && m2MoreSpecific)
                throw new Exception("Neither method is more specific than the other");

            return m1MoreSpecific ? m1 : m2;
        }

        public object Dispatch(object target, object[] arguments)
        {
            if (arguments[0] == null)
            {
                if (methods.TryGetValue(null, out var nullMethod))
                {
                    return nullMethod.Invoke(target, arguments);
                }
                // TODO throw exception about not having a null or default case
            }

            if (methods.TryGetValue(arguments[0].GetType(), out var method))
                return method.Invoke(target, arguments);

            throw new NotImplementedException();
        }
    }
}
