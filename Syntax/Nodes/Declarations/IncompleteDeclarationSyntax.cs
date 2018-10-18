using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class IncompleteDeclarationSyntax : DeclarationSyntax
    {
        [NotNull] public override IIdentifierToken Name => throw new NotImplementedException();

        [NotNull] [ItemNotNull] public IReadOnlyList<IToken> Tokens { get; }

        public IncompleteDeclarationSyntax([NotNull][ItemNotNull] IEnumerable<IToken> tokens)
        {
            Tokens = tokens.ToList().AsReadOnly().AssertNotNull();
        }
    }
}
