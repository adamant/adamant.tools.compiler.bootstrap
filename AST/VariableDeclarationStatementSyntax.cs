using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class VariableDeclarationStatementSyntax : StatementSyntax, ISymbol
    {
        public bool MutableBinding { get; }
        [NotNull] public SimpleName Name { get; }
        public TextSpan NameSpan { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        public Name FullName => throw new System.NotImplementedException();

        DataType ISymbol.Type => Type.Resolved();

        public ISymbol ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        public ISymbol Lookup(SimpleName name)
        {
            throw new System.NotImplementedException();
        }

        public VariableDeclarationStatementSyntax(
            bool mutableBinding,
            [NotNull] string name,
            TextSpan nameSpan,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
        {
            MutableBinding = mutableBinding;
            Name = new SimpleName(name);
            NameSpan = nameSpan;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
