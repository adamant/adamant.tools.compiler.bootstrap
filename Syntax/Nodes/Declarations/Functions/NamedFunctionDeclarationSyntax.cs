using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Effects;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public FunctionKeywordToken FunctionKeyword { get; }
        [NotNull] public override IIdentifierToken Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [NotNull] public IRightArrowToken Arrow { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }

        public NamedFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] FunctionKeywordToken functionKeyword,
            [NotNull] IIdentifierToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenToken closeParen,
            [NotNull] IRightArrowToken arrow,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [CanBeNull] BlockSyntax body,
            [CanBeNull] ISemicolonToken semicolon)
            : base(TextSpan.Covering(functionKeyword.Span, name.Span), modifiers,
            openParen, parameterList, closeParen, effects, contracts, body, semicolon)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(functionKeyword), functionKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(arrow), arrow);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            FunctionKeyword = functionKeyword;
            Name = name;
            GenericParameters = genericParameters;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
            GenericConstraints = genericConstraints;
        }
    }
}
