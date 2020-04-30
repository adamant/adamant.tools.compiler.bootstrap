using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    /// <summary>
    /// Maintains a list of places for a given function
    /// </summary>
    internal class PlaceList
    {
        private readonly Dictionary<SimpleName, VariablePlace> variables = new Dictionary<SimpleName, VariablePlace>();
        private readonly Dictionary<ISyntax, ObjectPlace> objects = new Dictionary<ISyntax, ObjectPlace>();

        /// <summary>
        /// Get or create a place for the named variable
        /// </summary>
        public VariablePlace GetOrAddVariable(SimpleName name)
        {
            if (!variables.TryGetValue(name, out var variable))
            {
                variable = new VariablePlace(name);
                variables.Add(name, variable);
            }

            return variable;
        }

        /// <summary>
        /// Get or create a place for the object passed into a parameter
        /// </summary>
        public ObjectPlace GetOrAddObject(IParameterSyntax parameter)
        {
            if (!objects.TryGetValue(parameter, out var @object))
            {
                @object = new ObjectPlace(parameter);
                objects.Add(parameter, @object);
            }

            return @object;
        }

        /// <summary>
        /// Get or create a place for the object produced by the expression
        /// </summary>
        public ObjectPlace GetOrAddObject(IExpressionSyntax expression)
        {
            if (!objects.TryGetValue(expression, out var @object))
            {
                @object = new ObjectPlace(expression);
                objects.Add(expression, @object);
            }

            return @object;
        }
    }
}
