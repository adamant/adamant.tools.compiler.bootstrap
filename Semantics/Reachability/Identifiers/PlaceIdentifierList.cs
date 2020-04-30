using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers
{
    /// <summary>
    /// Maintains a set of place identifiers for variables and objects
    /// </summary>
    internal class PlaceIdentifierList
    {
        private readonly Dictionary<SimpleName, VariablePlaceIdentifier> variableIdentifiers = new Dictionary<SimpleName, VariablePlaceIdentifier>();
        private readonly Dictionary<ISyntax, ObjectPlaceIdentifier> objects = new Dictionary<ISyntax, ObjectPlaceIdentifier>();

        /// <summary>
        /// Get or create a place for the named variable
        /// </summary>
        public VariablePlaceIdentifier VariableIdentifierFor(SimpleName variableName)
        {
            if (!variableIdentifiers.TryGetValue(variableName, out var variable))
            {
                variable = new VariablePlaceIdentifier(variableName);
                variableIdentifiers.Add(variableName, variable);
            }

            return variable;
        }

        /// <summary>
        /// Get or create a place for the object passed into a parameter
        /// </summary>
        public ObjectPlaceIdentifier ObjectIdentifierFor(IParameterSyntax parameter)
        {
            if (!objects.TryGetValue(parameter, out var @object))
            {
                @object = new ObjectPlaceIdentifier(parameter);
                objects.Add(parameter, @object);
            }

            return @object;
        }

        /// <summary>
        /// Get or create a place for the object produced by the expression
        /// </summary>
        public ObjectPlaceIdentifier ObjectIdentifierFor(IExpressionSyntax expression)
        {
            if (!objects.TryGetValue(expression, out var @object))
            {
                @object = new ObjectPlaceIdentifier(expression);
                objects.Add(expression, @object);
            }

            return @object;
        }
    }
}
