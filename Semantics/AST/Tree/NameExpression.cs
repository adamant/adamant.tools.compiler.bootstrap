using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class NameExpression : Expression, INameExpression
    {
        public NamedBindingSymbol ReferencedSymbol { get; }
        public Promise<bool> VariableIsLiveAfter { get; } = new Promise<bool>();

        public NameExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            NamedBindingSymbol referencedSymbol)
            : base(span, dataType, semantics)
        {
            ReferencedSymbol = referencedSymbol;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return ReferencedSymbol.Name.ToString();
        }
    }
}
