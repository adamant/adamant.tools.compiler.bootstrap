using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {
        public Token Binding { get; }
        public IdentifierToken Name { get; }
        public TypeSyntax Type { get; }
        public bool HasInitializer { get; }
        public ExpressionSyntax Initializer { get; }

        public VariableDeclarationStatementSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Binding = Children.OfType<Token>().First();
            Name = Children.OfType<IdentifierToken>().Single();
            Type = Children.OfType<TypeSyntax>().SingleOrDefault();
            HasInitializer = Children.OfType<Token>().Any(t => t.Kind == TokenKind.Equals);
            if (HasInitializer)
                Initializer = Children.OfType<ExpressionSyntax>().Last();
        }
    }
}
