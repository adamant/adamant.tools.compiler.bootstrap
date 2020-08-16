using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FieldAccessExpression : Expression, IFieldAccessExpression
    {
        public IExpression Context { get; }
        public AccessOperator AccessOperator { get; }
        public FieldSymbol ReferencedSymbol { get; }

        public FieldAccessExpression(
            TextSpan span,
            DataType dataType,
            IExpression context,
            AccessOperator accessOperator,
            FieldSymbol referencedSymbol)
            : base(span, dataType)
        {
            Context = context;
            AccessOperator = accessOperator;
            ReferencedSymbol = referencedSymbol;
        }
    }
}
