using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumValuePatternSyntax : PatternSyntax
    {
        [NotNull] public IIdentifierToken Identifier { get; }

        public EnumValuePatternSyntax([NotNull] IIdentifierToken identifier)
        {
            Identifier = identifier;
        }
    }
}
