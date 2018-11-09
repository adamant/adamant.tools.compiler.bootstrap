using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class AnyPatternSyntax : PatternSyntax
    {
        [NotNull] public IIdentifierToken Identifier { get; }

        public AnyPatternSyntax([NotNull] IIdentifierToken identifier)
        {
            Requires.NotNull(nameof(identifier), identifier);
            Identifier = identifier;
        }
    }
}
