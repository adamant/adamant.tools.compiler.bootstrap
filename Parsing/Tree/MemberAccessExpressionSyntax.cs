using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MemberAccessExpressionSyntax : ExpressionSyntax, IMemberAccessExpressionSyntax
    {
        private IExpressionSyntax expression;

        /// <summary>
        /// This expression is null for implicit member access i.e. self and enums
        /// </summary>
        public ref IExpressionSyntax? Expression => ref expression;

        public AccessOperator AccessOperator { get; }
        public INameSyntax Member { get; }

        [DisallowNull]
        public ISymbol? ReferencedSymbol
        {
            get => Member.ReferencedSymbol;
            set => Member.ReferencedSymbol = value;
        }

        public MemberAccessExpressionSyntax(
            TextSpan span,
            IExpressionSyntax? expression,
            AccessOperator accessOperator,
            INameSyntax member)
            : base(span)
        {
            this.expression = expression;
            AccessOperator = accessOperator;
            Member = member;
        }

        public override string ToString()
        {
            return $"{Expression}.{Member}";
        }
    }
}
