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
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        public TextSpan NameSpan { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; set; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

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
            throw new System.NotImplementedException();
        }
    }
}
