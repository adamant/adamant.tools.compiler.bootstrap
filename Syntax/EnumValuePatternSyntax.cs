using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumValuePatternSyntax : PatternSyntax
    {
        [NotNull]
        public IDotToken DotToken { get; }

        [NotNull]
        public IIdentifierTokenPlace Identifier { get; }

        public EnumValuePatternSyntax(
            [NotNull] IDotToken dotToken,
            [NotNull] IIdentifierTokenPlace identifier)
        {
            Requires.NotNull(nameof(dotToken), dotToken);
            Requires.NotNull(nameof(identifier), identifier);
            DotToken = dotToken;
            Identifier = identifier;
        }
    }
}
