using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterSyntax : SyntaxNode
    {
        [CanBeNull]
        public VarKeywordToken VarKeyword { get; }

        public IdentifierToken Name { get; }

        [CanBeNull]
        public ColonToken Colon { get; }
        public ExpressionSyntax TypeExpression { get; }

        public ParameterSyntax(
            [CanBeNull] VarKeywordToken varKeyword,
            IdentifierToken name,
            [CanBeNull] ColonToken colon,
            ExpressionSyntax typeExpression)
        {
            VarKeyword = varKeyword;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
        }
    }
}
