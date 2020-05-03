using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A graph of the possible references between places in a function. Also
    /// answers questions about the current mutability of objects etc.
    /// </summary>
    internal class ReachabilityGraph
    {
        private readonly LexicalVariableScope currentScope;
        private readonly Dictionary<IParameterSyntax, ContextObject> contextObjects = new Dictionary<IParameterSyntax, ContextObject>();
        private readonly Dictionary<IBindingSymbol, Variable> variables = new Dictionary<IBindingSymbol, Variable>();
        private readonly Dictionary<ISyntax, Object> objects = new Dictionary<ISyntax, Object>();
        private readonly HashSet<TempValue> tempValues = new HashSet<TempValue>();

        public ReachabilityGraph(LexicalVariableScope currentScope)
        {
            this.currentScope = currentScope;
        }

        private ContextObject ContextObjectFor(IParameterSyntax parameter)
        {
            if (!contextObjects.TryGetValue(parameter, out var place))
            {
                place = new ContextObject(parameter);
                contextObjects.Add(place.ForParameter, place);
            }

            return place;
        }

        public Variable ContextVariableAndObjectFor(IParameterSyntax parameter)
        {
            var callerVariable = currentScope.CallerScope.CallerVariable(parameter);
            var contextObject = ContextObjectFor(parameter);
            callerVariable.Owns(contextObject, true);
            return callerVariable;
        }

        public Variable VariableDeclared(IBindingSymbol variableSymbol)
        {
            currentScope.VariableDeclared(variableSymbol);
            var place = new Variable(variableSymbol);
            variables.Add(place.Symbol, place);
            return place;
        }
        public Variable VariableFor(IBindingSymbol variableSymbol)
        {
            // Variable needs to have already been declared
            return variables[variableSymbol];
        }

        public Variable? TryVariableFor(IBindingSymbol variableSymbol)
        {
            return variables.TryGetValue(variableSymbol, out var variable)
                ? variable : null;
        }

        public Object ObjectFor(IParameterSyntax parameter)
        {
            if (!objects.TryGetValue(parameter, out var place))
            {
                place = new Object(parameter);
                objects.Add(place.OriginSyntax, place);
            }

            return place;
        }
        public Object ObjectFor(IExpressionSyntax expression)
        {
            if (!objects.TryGetValue(expression, out var place))
            {
                place = new Object(expression);
                objects.Add(place.OriginSyntax, place);
            }

            return place;
        }

        public TempValue NewTempValue(ReferenceType referenceType)
        {
            var temp = new TempValue(referenceType);
            tempValues.Add(temp);
            return temp;
        }
    }
}
