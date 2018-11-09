using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class IncompleteDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] [ItemNotNull] public IReadOnlyList<ITokenPlace> Tokens { get; }

        public IncompleteDeclarationSyntax([NotNull][ItemNotNull] IReadOnlyList<ITokenPlace> tokens)
            : base(TextSpan.Covering(tokens.Select(t => t.Span).Cast<TextSpan?>().ToArray()))
        {
            Tokens = tokens.ToReadOnlyList();
        }
    }
}
