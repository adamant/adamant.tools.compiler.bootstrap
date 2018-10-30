using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class IncompleteDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] [ItemNotNull] public IReadOnlyList<IToken> Tokens { get; }

        public IncompleteDeclarationSyntax([NotNull][ItemNotNull] IEnumerable<IToken> tokens)
        {
            Tokens = tokens.ToReadOnlyList();
        }
    }
}
