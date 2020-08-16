using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class BlockExpression : Expression, IBlockExpression
    {
        public FixedList<IStatement> Statements { get; }

        public BlockExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            FixedList<IStatement> statements)
            : base(span, dataType, semantics)
        {
            Statements = statements;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            if (Statements.Any()) return $"{{ {Statements.Count} Statements }} : {DataType}";

            return $"{{ }} : {DataType}";
        }
    }
}
