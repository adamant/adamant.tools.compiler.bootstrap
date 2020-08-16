using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FunctionInvocationExpression : Expression, IFunctionInvocationExpression
    {
        public FunctionSymbol ReferencedSymbol { get; }
        public FixedList<IExpression> Arguments { get; }

        public FunctionInvocationExpression(TextSpan span, DataType dataType, FunctionSymbol referencedSymbol, FixedList<IExpression> arguments)
            : base(span, dataType)
        {
            ReferencedSymbol = referencedSymbol;
            Arguments = arguments;
        }
    }
}
