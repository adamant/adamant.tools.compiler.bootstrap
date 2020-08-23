using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class SelfParameterName : AbstractSyntax, ISelfParameterName
    {
        public SelfParameterSymbol ReferencedSymbol { get; }
        BindingSymbol IParameterName.ReferencedSymbol => ReferencedSymbol;

        public SelfParameterName(TextSpan span, SelfParameterSymbol referencedSymbol)
            : base(span)
        {
            ReferencedSymbol = referencedSymbol;
        }

        public override string ToString()
        {
            return "self";
        }
    }
}
