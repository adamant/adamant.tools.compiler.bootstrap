using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Patterns
{
    public class AnyPatternSyntax : PatternSyntax
    {
        [NotNull] public IdentifierToken Identifier { get; }

        public AnyPatternSyntax([NotNull] IdentifierToken identifier)
        {
            Requires.NotNull(nameof(identifier), identifier);
            Identifier = identifier;
        }
    }
}
