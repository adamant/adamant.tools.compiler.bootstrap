using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public AccessModifierSyntax AccessModifier { get; }
        [NotNull] public IFunctionKeywordToken FunctionKeyword { get; }
        [NotNull] public override IIdentifierToken Name { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ParameterSyntax> ParametersList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ParameterSyntax> Parameters => ParametersList.Nodes();
        [NotNull] public ICloseParenToken CloseParen { get; }
        [NotNull] public IRightArrowToken Arrow { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }
        [CanBeNull] public EffectsSyntax Effects { get; }
        [NotNull] public BlockExpressionSyntax Body { get; }

        public FunctionDeclarationSyntax(
            [NotNull] AccessModifierSyntax accessModifier,
            [NotNull] IFunctionKeywordToken functionKeyword,
            [NotNull] IIdentifierToken name,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parametersList,
            [NotNull] ICloseParenToken closeParen,
            [NotNull] IRightArrowToken arrow,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] BlockExpressionSyntax body)
        {
            Requires.NotNull(nameof(accessModifier), accessModifier);
            Requires.NotNull(nameof(parametersList), parametersList);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            Requires.NotNull(nameof(body), body);
            AccessModifier = accessModifier;
            FunctionKeyword = functionKeyword;
            Name = name;
            OpenParen = openParen;
            ParametersList = parametersList;
            CloseParen = closeParen;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
            Effects = effects;
            Body = body;
        }
    }
}
