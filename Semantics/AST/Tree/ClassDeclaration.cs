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
        public ConstructorSymbol? DefaultConstructorSymbol { get; }

        public ClassDeclaration(
            CodeFile file,
            TextSpan span,
            ObjectTypeSymbol symbol,
            TextSpan nameSpan,
            ConstructorSymbol? defaultConstructorSymbol,
            Func<IClassDeclaration, FixedList<IMemberDeclaration>> buildMembers)
            : base(file, span, symbol, nameSpan)
        {
            Symbol = symbol;
            DefaultConstructorSymbol = defaultConstructorSymbol;
            Members = buildMembers(this);
        }

        public override string ToString()
        {
            return $"class {Symbol.ContainingSymbol}.{Symbol.Name} {{ … }}";
        }
    }
}
