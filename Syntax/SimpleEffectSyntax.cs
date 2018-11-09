using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SimpleEffectSyntax : EffectSyntax
    {
        [NotNull] public IIdentifierToken Identifier { get; }

        public SimpleEffectSyntax([NotNull] IIdentifierToken identifier)
        {
            Identifier = identifier;
        }
    }
}
