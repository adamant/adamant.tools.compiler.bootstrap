using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class EnumValuePatternSyntax : PatternSyntax
    {
        [NotNull]
        public DotToken DotToken { get; }

        [NotNull]
        public IIdentifierToken Identifier { get; }

        public EnumValuePatternSyntax(
            [NotNull] DotToken dotToken,
            [NotNull] IIdentifierToken identifier)
        {
            Requires.NotNull(nameof(dotToken), dotToken);
            Requires.NotNull(nameof(identifier), identifier);
            DotToken = dotToken;
            Identifier = identifier;
        }
    }
}
