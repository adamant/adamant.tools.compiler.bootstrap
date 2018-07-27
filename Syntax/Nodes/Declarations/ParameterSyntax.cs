using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ParameterSyntax : DeclarationSyntax
    {
        public ParameterSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }

        public Token VarKeyword => Children.OfType<Token>().Where(t => t.Kind == TokenKind.VarKeyword).SingleOrDefault();
        public IdentifierToken Identifier => Children.OfType<IdentifierToken>().Single();
        public TypeSyntax Type => Children.OfType<TypeSyntax>().Single();
    }
}
