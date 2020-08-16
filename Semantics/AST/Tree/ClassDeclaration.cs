using System;
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
            CodeFile file,
            TextSpan span,
            ObjectTypeSymbol symbol,
            Func<IClassDeclaration, FixedList<IMemberDeclaration>> buildMembers)
            : base(file, span, symbol)
        {
            Symbol = symbol;
            Members = buildMembers(this);
        }

        public override string ToString()
        {
            return $"class {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
        }
    }
}
