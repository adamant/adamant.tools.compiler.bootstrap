using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ForeachExpressionSyntax : ExpressionSyntax, IForeachExpressionSyntax
    {
        public bool IsMutableBinding { get; }
        public Name VariableName { get; }
        public Promise<int?> DeclarationNumber { get; } = new Promise<int?>();
        public Promise<VariableSymbol> Symbol { get; } = new Promise<VariableSymbol>();
        IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
        public bool VariableIsLiveAfterAssignment { get; set; } = true;

        public ITypeSyntax? Type { get; }
        private IExpressionSyntax inExpression;
        public ref IExpressionSyntax InExpression => ref inExpression;

        public IBlockExpressionSyntax Block { get; }

        public ForeachExpressionSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name variableName,
            ITypeSyntax? typeSyntax,
            IExpressionSyntax inExpression,
            IBlockExpressionSyntax block)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            VariableName = variableName;
            this.inExpression = inExpression;
            Block = block;
            Type = typeSyntax;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            var binding = IsMutableBinding ? "var " : "";
            return $"foreach {binding}{VariableName}: {Type} in {InExpression} {Block}";
        }
    }
}
