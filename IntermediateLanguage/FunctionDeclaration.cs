using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FunctionDeclaration : Declaration, IInvocableDeclaration
    {
        public bool IsExternal { get; }

        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool IInvocableDeclaration.IsConstructor => false;

        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public new FunctionSymbol Symbol { get; }
        public ControlFlowGraph IL { get; }

        public FunctionDeclaration(
            bool isExternal,
            bool isMember,
            MaybeQualifiedName name,
            FixedList<Parameter> parameters,
            FunctionSymbol symbol,
            ControlFlowGraph il)
            : base(isMember, name, symbol)
        {
            Parameters = parameters;
            Symbol = symbol;
            IL = il;
            IsExternal = isExternal;
        }
    }
}
