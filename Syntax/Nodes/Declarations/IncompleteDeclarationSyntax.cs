using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class IncompleteDeclarationSyntax : DeclarationSyntax
    {
        public override IdentifierToken Name { get; }
        public IReadOnlyList<Token> Tokens { get; }

        public IncompleteDeclarationSyntax(IdentifierToken name, IEnumerable<Token> tokens)
        {
            Name = name;
            Tokens = tokens.ToList().AsReadOnly();
        }
    }
}
