using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts
{
    public class ParameterSyntax : SyntaxNode
    {
        [CanBeNull] public VarKeywordToken VarKeyword { get; }
        [CanBeNull] public IdentifierToken Name { get; }
        [CanBeNull] public ColonToken Colon { get; }
        [NotNull] public ExpressionSyntax TypeExpression { get; }

        public ParameterSyntax(
            [CanBeNull] VarKeywordToken varKeyword,
            [CanBeNull] IdentifierToken name,
            [CanBeNull] ColonToken colon,
            [NotNull] ExpressionSyntax typeExpression)
        {
            Requires.NotNull(nameof(typeExpression), typeExpression);
            VarKeyword = varKeyword;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
        }
    }
}
