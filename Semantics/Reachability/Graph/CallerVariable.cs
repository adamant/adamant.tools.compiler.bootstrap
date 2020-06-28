using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A caller variable is a variable that exists in the caller's stackframe
    /// </summary>
    internal class CallerVariable : StackPlace
    {
        public IBindingSymbol Symbol { get; }

        private CallerVariable(IBindingSymbol symbol)
        {
            Symbol = symbol;
            _ = symbol.Type.UnderlyingReferenceType()
                   ?? throw new ArgumentException("Must be a reference type", nameof(symbol));
        }

        public static CallerVariable ForParameterWithObject(IParameterSyntax parameter)
        {
            var reference = Reference.ToNewParameterContextObject(parameter);
            var callerVariable = new CallerVariable(parameter);
            callerVariable.AddReference(reference);
            return callerVariable;
        }
    }
}
