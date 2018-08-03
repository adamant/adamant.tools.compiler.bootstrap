using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ParameterSyntax : DeclarationSyntax
    {
        public Token VarKeyword { get; }
        public override IdentifierToken Name { get; }
        public TypeSyntax Type { get; }

        public ParameterSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            VarKeyword = Children.OfType<Token>().SingleOrDefault(t => t.Kind == TokenKind.VarKeyword);
            Name = Children.OfType<IdentifierToken>().Single();
            Type = Children.OfType<TypeSyntax>().Single();
        }
    }
}
