using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull]
        public AccessModifierSyntax AccessModifier { get; }

        [CanBeNull]
        public FunctionKeywordToken FunctionKeyword { get; }

        [CanBeNull]
        public override IdentifierToken Name { get; }

        [CanBeNull]
        public OpenParenToken OpenParen { get; }

        [NotNull]
        public SeparatedListSyntax<ParameterSyntax> Parameters { get; }

        [CanBeNull]
        public CloseParenToken CloseParen { get; }

        [CanBeNull]
        public RightArrowToken Arrow { get; }

        public ExpressionSyntax ReturnTypeExpression { get; }

        public BlockStatementSyntax Body { get; }

        public FunctionDeclarationSyntax(
            [NotNull] AccessModifierSyntax accessModifier,
            [CanBeNull] FunctionKeywordToken functionKeyword,
            [CanBeNull] IdentifierToken name,
            [CanBeNull] OpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameters,
            [CanBeNull] CloseParenToken closeParen,
            [CanBeNull] RightArrowToken arrow,
            ExpressionSyntax returnTypeExpression,
            BlockStatementSyntax body)
        {
            Requires.NotNull(nameof(accessModifier), accessModifier);
            Requires.NotNull(nameof(parameters), parameters);
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
