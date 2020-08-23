using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class NamedParameterName : AbstractSyntax, INamedParameterName
    {
        public VariableSymbol ReferencedSymbol { get; }
        BindingSymbol IParameterName.ReferencedSymbol => ReferencedSymbol;

        public NamedParameterName(TextSpan span, VariableSymbol referencedSymbol)
            : base(span)
        {
            ReferencedSymbol = referencedSymbol;
        }

        public override string ToString()
        {
            return ReferencedSymbol.Name.ToString();
        }
    }
}
