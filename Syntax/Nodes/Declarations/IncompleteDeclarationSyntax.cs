using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class IncompleteDeclarationSyntax : DeclarationSyntax
    {
        [CanBeNull] public override IdentifierToken Name => null;

        [NotNull] [ItemNotNull] public IReadOnlyList<Token> Tokens { get; }

        public IncompleteDeclarationSyntax([NotNull][ItemNotNull] IEnumerable<Token> tokens)
        {
            Tokens = tokens.ToList().AsReadOnly().AssertNotNull();
        }
    }
}
