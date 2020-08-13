using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class SelfExpressionSyntax : ExpressionSyntax, ISelfExpressionSyntax
    {
        public bool IsImplicit { get; }
        public Promise<SelfParameterSymbol?> ReferencedSymbol { get; } = new Promise<SelfParameterSymbol?>();

        public SelfExpressionSyntax(TextSpan span, bool isImplicit)
            : base(span)
        {
            IsImplicit = isImplicit;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return IsImplicit ? "⟦self⟧" : "self";
        }
    }
}
