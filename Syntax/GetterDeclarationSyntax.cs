using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GetterDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IIdentifierToken Name { get; }
        [NotNull] public ExpressionSyntax ReturnType { get; }

        public GetterDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierToken name,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] ExpressionSyntax returnType,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(modifiers, name.Span, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            Name = name;
            ReturnType = returnType;
        }
    }
}
