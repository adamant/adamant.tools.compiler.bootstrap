using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldAccessExpressionSyntax : ExpressionSyntax, IFieldAccessExpressionSyntax
    {
        private IExpressionSyntax contextExpression;
        public ref IExpressionSyntax ContextExpression => ref contextExpression;

        public AccessOperator AccessOperator { get; }
        public INameExpressionSyntax Field { get; }

        [DisallowNull]
        public IBindingSymbol? ReferencedSymbol
        {
            get => Field.ReferencedSymbol;
            set
            {
                if (Field.ReferencedSymbol != null)
                    throw new InvalidOperationException("Can't set ReferencedSymbol repeatedly");
                Field.ReferencedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public FieldAccessExpressionSyntax(
            TextSpan span,
            IExpressionSyntax contextExpression,
            AccessOperator accessOperator,
            INameExpressionSyntax field)
            : base(span)
        {
            this.contextExpression = contextExpression;
            AccessOperator = accessOperator;
            Field = field;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{ContextExpression.ToGroupedString(ExpressionPrecedence)}.{Field}";
        }
    }
}
