using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class MoveExpression : Expression, IMoveExpression
    {
        public BindingSymbol ReferencedSymbol { get; }
        public IExpression Referent { get; }

        public MoveExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            BindingSymbol referencedSymbol,
            IExpression referent)
            : base(span, dataType, semantics)
        {
            ReferencedSymbol = referencedSymbol;
            Referent = referent;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return $"move {Referent}";
        }
    }
}
