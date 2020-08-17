using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FieldDeclaration : Declaration, IFieldDeclaration
    {
        public IClassDeclaration DeclaringClass { get; }
        public new FieldSymbol Symbol { get; }
        BindingSymbol IBinding.Symbol => Symbol;

        public FieldDeclaration(
            CodeFile file,
            TextSpan span,
            IClassDeclaration declaringClass,
            FieldSymbol symbol,
            TextSpan nameSpan)
            : base(file, span, symbol, nameSpan)
        {
            Symbol = symbol;
            DeclaringClass = declaringClass;
        }

        public override string ToString()
        {
            return Symbol + ";";
        }
    }
}
