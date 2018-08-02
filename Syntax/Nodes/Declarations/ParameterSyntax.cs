using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ParameterSyntax : DeclarationSyntax
    {
        public Token VarKeyword => Children.OfType<Token>().SingleOrDefault(t => t.Kind == TokenKind.VarKeyword);
        public override IdentifierToken Name => Children.OfType<IdentifierToken>().Single();
        public TypeSyntax Type => Children.OfType<TypeSyntax>().Single();

        public ParameterSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
