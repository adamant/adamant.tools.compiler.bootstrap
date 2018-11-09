using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
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
