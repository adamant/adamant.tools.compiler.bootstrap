using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class MethodInvocationExpression : Expression, IMethodInvocationExpression
    {
        public IExpression Context { get; }
        public MethodSymbol ReferencedSymbol { get; }
        public FixedList<IExpression> Arguments { get; }

        public MethodInvocationExpression(
            TextSpan span,
            DataType dataType,
            IExpression context,
            MethodSymbol referencedSymbol,
            FixedList<IExpression> arguments)
            : base(span, dataType)
        {
            Context = context;
            ReferencedSymbol = referencedSymbol;
            Arguments = arguments;
        }
    }
}
