using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {
        public Token Binding { get; }
        public IdentifierToken Name { get; }
        public TypeSyntax Type { get; }
        public bool HasInitializer => Initializer != null;
        public ExpressionSyntax Initializer { get; }

        public VariableDeclarationStatementSyntax(IEnumerable<SyntaxNode> children, TypeSyntax type, ExpressionSyntax initializer)
            : base(children)
        {
            Requires.That(nameof(type), type == null || Children.Contains(type));
            Requires.That(nameof(initializer), initializer == null || Children.Contains(initializer));
            Binding = Children.OfType<Token>().First();
            Name = Children.OfType<IdentifierToken>().Single();
            Type = type;
            Initializer = initializer;
        }
    }
}
