using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ForeachExpression : Expression, IForeachExpression
    {
        public VariableSymbol Symbol { get; }
        BindingSymbol IBinding.Symbol => Symbol;
        NamedBindingSymbol ILocalBinding.Symbol => Symbol;
        public IExpression InExpression { get; }
        public IBlockExpression Block { get; }


        public ForeachExpression(
            TextSpan span,
            DataType dataType,
            VariableSymbol symbol,
            IExpression inExpression,
            IBlockExpression block)
            : base(span, dataType)
        {
            Symbol = symbol;
            InExpression = inExpression;
            Block = block;
        }
    }
}
