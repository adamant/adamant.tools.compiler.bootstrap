using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
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
            ExpressionSemantics semantics,
            IExpression context,
            AccessOperator accessOperator,
            FieldSymbol referencedSymbol)
            : base(span, dataType, semantics)
        {
            Context = context;
            AccessOperator = accessOperator;
            ReferencedSymbol = referencedSymbol;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{Context.ToGroupedString(ExpressionPrecedence)}{AccessOperator.ToSymbolString()}{ReferencedSymbol.Name}";
        }
    }
}
