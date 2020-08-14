using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class MethodDeclaration : Declaration, IInvocableDeclaration
    {
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        bool IInvocableDeclaration.IsConstructor => false;
        public Parameter SelfParameter { get; }
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public new MethodSymbol Symbol { get; }
        public ControlFlowGraph? IL { get; }

        public MethodDeclaration(
            MaybeQualifiedName name,
            Parameter selfParameter,
            FixedList<Parameter> parameters,
            MethodSymbol symbol,
            ControlFlowGraph? il)
            : base(true, name, symbol)
        {
            SelfParameter = selfParameter;
            Parameters = parameters;
            Symbol = symbol;
            IL = il;
        }
    }
}
