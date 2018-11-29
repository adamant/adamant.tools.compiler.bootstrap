using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax, INamespacedDeclarationSyntax
    {
        [NotNull] public Name FullName { get; }
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;

        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        ISymbol ISymbol.Lookup(SimpleName name)
        {
            throw new System.NotImplementedException();
        }

        [CanBeNull] public ExpressionSyntax ReturnTypeExpression { get; }
        DataType ISymbol.Type => Type.Fulfilled();

        public NamedFunctionDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters, // For now we will not support pure meta functions
            [CanBeNull] ExpressionSyntax returnTypeExpression,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(file, modifiers, nameSpan, genericParameters, parameters,
                genericConstraints, mayEffects, noEffects, requires, ensures, body)
        {
            FullName = fullName;
            ReturnTypeExpression = returnTypeExpression;
        }

        public override string ToString()
        {
            var generics = GenericParameters != null
                ? $"[{string.Join(", ", GenericParameters)}]"
                : "";
            return $"fn {FullName}{generics}({string.Join(", ", Parameters)}) -> {ReturnTypeExpression} {{ … }}";
        }
    }
}
