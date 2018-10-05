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
        public SimpleToken Binding { get; }
        public IdentifierToken Name { get; }
        public TypeSyntax Type { get; }
        public bool HasInitializer => Initializer != null;
        public ExpressionSyntax Initializer { get; }

        public VariableDeclarationStatementSyntax(IEnumerable<SyntaxNode> children, TypeSyntax type, ExpressionSyntax initializer)
        {
            var childNodes = children.ToList();
            Requires.That(nameof(type), type == null || childNodes.Contains(type));
            Requires.That(nameof(initializer), initializer == null || childNodes.Contains(initializer));
            Binding = childNodes.OfType<SimpleToken>().First();
            Name = childNodes.OfType<IdentifierToken>().Single();
            Type = type;
            Initializer = initializer;
        }
    }
}
