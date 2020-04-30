using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldAccessExpressionSyntax : ExpressionSyntax, IFieldAccessExpressionSyntax
    {
        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression => ref expression;

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
            IExpressionSyntax expression,
            AccessOperator accessOperator,
            INameExpressionSyntax field)
            : base(span)
        {
            this.expression = expression;
            AccessOperator = accessOperator;
            Field = field;
        }

        public override string ToString()
        {
            return $"{Expression}.{Field}";
        }
    }
}
