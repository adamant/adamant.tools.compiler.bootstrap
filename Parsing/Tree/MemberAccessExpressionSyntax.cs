using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MemberAccessExpressionSyntax : ExpressionSyntax, IMemberAccessExpressionSyntax
    {
        private IExpressionSyntax? expression;

        [DisallowNull]
        public ref IExpressionSyntax? Expression => ref expression;

        public AccessOperator AccessOperator { get; }
        public INameExpressionSyntax Member { get; }

        [DisallowNull]
        public IBindingSymbol? ReferencedSymbol
        {
            get => Member.ReferencedSymbol;
            set
            {
                if (Member.ReferencedSymbol != null)
                    throw new InvalidOperationException("Can't set ReferencedSymbol repeatedly");
                Member.ReferencedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public MemberAccessExpressionSyntax(
            TextSpan span,
            IExpressionSyntax? expression,
            AccessOperator accessOperator,
            INameExpressionSyntax member)
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
