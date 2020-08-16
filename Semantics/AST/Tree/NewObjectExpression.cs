using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class NewObjectExpression : Expression, INewObjectExpression
    {
        public ConstructorSymbol ReferencedSymbol { get; }
        public FixedList<IExpression> Arguments { get; }

        public NewObjectExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            ConstructorSymbol referencedSymbol,
            FixedList<IExpression> arguments)
            : base(span, dataType, semantics)
        {
            ReferencedSymbol = referencedSymbol;
            Arguments = arguments;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            var name = ReferencedSymbol.Name != null ? "." + ReferencedSymbol.Name : "";
            return $"new {ReferencedSymbol.ContainingSymbol}{name}({string.Join(", ", Arguments)})";
        }
    }
}
