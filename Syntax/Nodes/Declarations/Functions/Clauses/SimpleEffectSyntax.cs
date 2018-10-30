using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Clauses
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