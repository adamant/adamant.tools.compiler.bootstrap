using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ClassDeclaration : Declaration, IClassDeclaration
    {
        public new ObjectTypeSymbol Symbol { get; }
        public FixedList<IMemberDeclaration> Members { get; }

        public ClassDeclaration(
            TextSpan span,
            ObjectTypeSymbol symbol,
            FixedList<IMemberDeclaration> members)
            : base(span, symbol)
        {
            Symbol = symbol;
            Members = members;
        }
    }
}
