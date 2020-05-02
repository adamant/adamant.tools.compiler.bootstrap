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
        private readonly Dictionary<SimpleName, FieldPlaceIdentifier> fieldIdentifiers = new Dictionary<SimpleName, FieldPlaceIdentifier>();
        private readonly Dictionary<ISyntax, ObjectPlaceIdentifier> objectIdentifiers = new Dictionary<ISyntax, ObjectPlaceIdentifier>();

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

        public FieldPlaceIdentifier FieldIdentifierFor(SimpleName fieldName)
        {
            if (!fieldIdentifiers.TryGetValue(fieldName, out var variable))
            {
                variable = new FieldPlaceIdentifier(fieldName);
                fieldIdentifiers.Add(fieldName, variable);
            }

            return variable;
        }

        /// <summary>
        /// Get or create a place for the object passed into a parameter
        /// </summary>
        public ObjectPlaceIdentifier ObjectIdentifierFor(IParameterSyntax parameter)
        {
            if (!objectIdentifiers.TryGetValue(parameter, out var @object))
            {
                @object = new ObjectPlaceIdentifier(parameter);
                objectIdentifiers.Add(parameter, @object);
            }

            return @object;
        }

        public ObjectPlaceIdentifier ObjectIdentifierFor(IConstructorDeclarationSyntax constructor)
        {
            if (!objectIdentifiers.TryGetValue(constructor, out var @object))
            {
                @object = new ObjectPlaceIdentifier(constructor);
                objectIdentifiers.Add(constructor, @object);
            }

            return @object;
        }

        /// <summary>
        /// Get or create a place for the object produced by the expression
        /// </summary>
        public ObjectPlaceIdentifier ObjectIdentifierFor(IExpressionSyntax expression)
        {
            if (!objectIdentifiers.TryGetValue(expression, out var @object))
            {
                @object = new ObjectPlaceIdentifier(expression);
                objectIdentifiers.Add(expression, @object);
            }

            return @object;
        }


    }
}
