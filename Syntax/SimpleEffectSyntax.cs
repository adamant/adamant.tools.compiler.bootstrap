using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SimpleEffectSyntax : EffectSyntax
    {
        [NotNull] public IdentifierToken Identifier { get; }

        public SimpleEffectSyntax([NotNull] IdentifierToken identifier)
        {
            Identifier = identifier;
        }
    }
}
