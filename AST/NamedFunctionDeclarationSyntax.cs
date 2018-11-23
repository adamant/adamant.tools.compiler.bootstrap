using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax, INamespacedDeclarationSyntax
    {
        [NotNull] public SimpleName Name { get; }

        IEnumerable<DataType> ISymbol.Types => throw new System.NotImplementedException();

        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        ISymbol ISymbol.Lookup(SimpleName name)
        {
            throw new System.NotImplementedException();
        }

        [CanBeNull] public ExpressionSyntax ReturnTypeExpression { get; }

        public NamedFunctionDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] string name,
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
            Name = new SimpleName(name);
            ReturnTypeExpression = returnTypeExpression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }

        Name ISymbol.Name => Name;
    }
}
