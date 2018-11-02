using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Effects;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions
{
    public class InitializerFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public InitKeywordToken InitKeyword { get; }
        [CanBeNull] public override IIdentifierToken Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }

        public InitializerFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] InitKeywordToken initKeyword,
            [CanBeNull] IdentifierToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenToken closeParen,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [NotNull] BlockSyntax body)
            : base(TextSpan.Covering(initKeyword.Span, name?.Span), modifiers,
            openParen, parameterList, closeParen, effects, contracts, body, null)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(initKeyword), initKeyword);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(contracts), contracts);
            Requires.NotNull(nameof(body), body);
            InitKeyword = initKeyword;
            Name = name;
            GenericParameters = genericParameters;
            GenericConstraints = genericConstraints;
        }
    }
}
