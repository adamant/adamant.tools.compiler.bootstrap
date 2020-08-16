using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FieldDeclaration : Declaration, IFieldDeclaration
    {
        public new FieldSymbol Symbol { get; }
        BindingSymbol IBinding.Symbol => Symbol;

        public FieldDeclaration(TextSpan span, FieldSymbol symbol)
            : base(span, symbol)
        {
            Symbol = symbol;
        }
    }
}
