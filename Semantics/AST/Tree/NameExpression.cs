using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class NameExpression : Expression, INameExpression
    {
        public NamedBindingSymbol ReferencedSymbol { get; }

        public NameExpression(
            TextSpan span,
            DataType dataType,
            NamedBindingSymbol referencedSymbol)
            : base(span, dataType)
        {
            ReferencedSymbol = referencedSymbol;
        }
    }
}
