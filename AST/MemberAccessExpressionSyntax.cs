using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        /// <summary>
        /// This expression is null for implicit member access i.e. self and enums
        /// </summary>
        public ExpressionSyntax Expression;
        public AccessOperator AccessOperator { get; }
        public NameSyntax Member { get; }

        public ISymbol? ReferencedSymbol
        {
            get => Member.ReferencedSymbol;
            set => Member.ReferencedSymbol = value;
        }

        public MemberAccessExpressionSyntax(
            TextSpan span,
            ExpressionSyntax expression,
            AccessOperator accessOperator,
            NameSyntax member)
            : base(span)
        {
            Expression = expression;
            AccessOperator = accessOperator;
            Member = member;
        }

        public override string ToString()
        {
            return $"{Expression}.{Member}";
        }
    }
}
