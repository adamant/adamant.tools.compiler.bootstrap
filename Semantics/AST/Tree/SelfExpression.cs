using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class SelfExpression : Expression, ISelfExpression
    {
        public SelfParameterSymbol ReferencedSymbol { get; }
        public bool IsImplicit { get; }

        public SelfExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            SelfParameterSymbol referencedSymbol,
            bool isImplicit)
            : base(span, dataType, semantics)
        {
            ReferencedSymbol = referencedSymbol;
            IsImplicit = isImplicit;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return IsImplicit ? "⟦self⟧" : "self";
        }
    }
}
