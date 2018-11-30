using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class VariableDeclarationStatementSyntax : StatementSyntax, ISymbol
    {
        public bool MutableBinding { get; }
        [NotNull] public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName LookupByName => FullName.UnqualifiedName.WithoutNumber();
        public TextSpan NameSpan { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; set; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ISymbol.Type => Type.Fulfilled();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public VariableDeclarationStatementSyntax(
            bool mutableBinding,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
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
