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
        [NotNull] public SeparatedListSyntax<ParameterSyntax> Parameters { get; }
        [NotNull] public ICloseParenToken CloseParen { get; }
        [NotNull] public IRightArrowToken Arrow { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }
        [NotNull] public BlockExpressionSyntax Body { get; }

        public FunctionDeclarationSyntax(
            [NotNull] AccessModifierSyntax accessModifier,
            [NotNull] IFunctionKeywordToken functionKeyword,
            [NotNull] IIdentifierToken name,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameters,
            [NotNull] ICloseParenToken closeParen,
            [NotNull] IRightArrowToken arrow,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] BlockExpressionSyntax body)
        {
            Requires.NotNull(nameof(accessModifier), accessModifier);
            Requires.NotNull(nameof(parameters), parameters);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            Requires.NotNull(nameof(body), body);
            AccessModifier = accessModifier;
            FunctionKeyword = functionKeyword;
            Name = name;
            OpenParen = openParen;
            Parameters = parameters;
            CloseParen = closeParen;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
            Body = body;
        }
    }
}
