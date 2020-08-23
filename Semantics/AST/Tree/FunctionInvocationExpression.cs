using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FunctionInvocationExpression : Expression, IFunctionInvocationExpression
    {
        public FunctionSymbol ReferencedSymbol { get; }
        InvocableSymbol IInvocationExpression.ReferencedSymbol => ReferencedSymbol;
        public FixedList<IExpression> Arguments { get; }

        public FunctionInvocationExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            FunctionSymbol referencedSymbol,
            FixedList<IExpression> arguments)
            : base(span, dataType, semantics)
        {
            ReferencedSymbol = referencedSymbol;
            Arguments = arguments;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{ReferencedSymbol.ContainingSymbol}.{ReferencedSymbol.Name}({string.Join(", ", Arguments)})";
        }
    }
}
