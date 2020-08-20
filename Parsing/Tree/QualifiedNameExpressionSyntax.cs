using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class QualifiedNameExpressionSyntax : ExpressionSyntax, IQualifiedNameExpressionSyntax
    {
        private IExpressionSyntax context;
        public ref IExpressionSyntax Context => ref context;

        public AccessOperator AccessOperator { get; }
        public INameExpressionSyntax Field { get; }
        public IPromise<FieldSymbol?> ReferencedSymbol => Field.ReferencedSymbol.Select(s => (FieldSymbol?)s);

        public QualifiedNameExpressionSyntax(
            TextSpan span,
            IExpressionSyntax context,
            AccessOperator accessOperator,
            INameExpressionSyntax field)
            : base(span)
        {
            this.context = context;
            AccessOperator = accessOperator;
            Field = field;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{Context.ToGroupedString(ExpressionPrecedence)}{AccessOperator.ToSymbolString()}{Field}";
        }
    }
}
