using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class VariableDeclarationStatementSyntax : StatementSyntax, IBindingSymbol
    {
        public bool IsMutableBinding { get; }
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public TextSpan NameSpan { get; }
        public TypeSyntax TypeSyntax { get; }
        public ExpressionSyntax Initializer;

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public VariableDeclarationStatementSyntax(
            bool isMutableBinding,
            Name fullName,
            TextSpan nameSpan,
            TypeSyntax typeSyntax,
            ExpressionSyntax initializer)
        {
            IsMutableBinding = isMutableBinding;
            FullName = fullName;
            NameSpan = nameSpan;
            TypeSyntax = typeSyntax;
            Initializer = initializer;
        }

        public override string ToString()
        {
            var binding = IsMutableBinding ? "var" : "let";
            var type = TypeSyntax != null ? ": " + TypeSyntax : "";
            var initializer = Initializer != null ? " = " + Initializer : "";
            return $"{binding} {Name}{type}{initializer};";
        }
    }
}
