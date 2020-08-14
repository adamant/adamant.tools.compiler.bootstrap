using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class MethodDeclarationIL : DeclarationIL, IInvocableDeclarationIL
    {
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        bool IInvocableDeclarationIL.IsConstructor => false;
        public ParameterIL SelfParameter { get; }
        public FixedList<ParameterIL> Parameters { get; }
        public int Arity => Parameters.Count;
        public new MethodSymbol Symbol { get; }
        public ControlFlowGraph? IL { get; }

        public MethodDeclarationIL(
            MethodSymbol symbol,
            ParameterIL selfParameter,
            FixedList<ParameterIL> parameters,
            ControlFlowGraph? il)
            : base(true, symbol)
        {
            SelfParameter = selfParameter;
            Parameters = parameters;
            Symbol = symbol;
            IL = il;
        }
    }
}
