using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Effects
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
