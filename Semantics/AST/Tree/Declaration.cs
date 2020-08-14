using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class Declaration : AbstractSyntax, IDeclaration
    {
        public Symbol Symbol { get; }

        protected Declaration(TextSpan span, Symbol symbol)
            : base(span)
        {
            Symbol = symbol;
        }
    }
}
