using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public abstract IIdentifierTokenPlace Name { get; }
        [CanBeNull] public FixedList<ParameterSyntax> Parameters { get; }
        [NotNull] public FixedList<EffectSyntax> MayEffects { get; }
        [NotNull] public FixedList<EffectSyntax> NoEffects { get; }
        [NotNull] public FixedList<ExpressionSyntax> Requires { get; }
        [NotNull] public FixedList<ExpressionSyntax> Ensures { get; }
        [CanBeNull] public BlockSyntax Body { get; }

        protected FunctionDeclarationSyntax(
            TextSpan nameSpan,
            [NotNull] FixedList<IModiferToken> modifiers,
            [CanBeNull] FixedList<ParameterSyntax> parameters,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(nameSpan)
        {
            Modifiers = modifiers;
            Parameters = parameters;
            MayEffects = mayEffects;
            NoEffects = noEffects;
            Requires = requires;
            Ensures = ensures;
            Body = body;
        }
    }
}
