using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class VariableDeclarationStatementSyntax : StatementSyntax, ISymbol
    {
        public bool MutableBinding { get; }
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public TextSpan NameSpan { get; }
        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax Initializer { get; set; }
        public TypePromise Type { get; } = new TypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ISymbol.Type => Type.Fulfilled();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public VariableDeclarationStatementSyntax(
            bool mutableBinding,
            Name fullName,
            TextSpan nameSpan,
            ExpressionSyntax typeExpression,
            ExpressionSyntax initializer)
        {
            MutableBinding = mutableBinding;
            FullName = fullName;
            NameSpan = nameSpan;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        public override string ToString()
        {
            var binding = MutableBinding ? "var" : "let";
            var type = TypeExpression != null ? ": " + TypeExpression : "";
            var initializer = Initializer != null ? " = " + Initializer : "";
            return $"{binding} {Name}{type}{initializer};";
        }
    }
}
