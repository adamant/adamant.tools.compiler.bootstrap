using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BlockExpressionSyntax : ExpressionSyntax, IBlockExpressionSyntax
    {
        public FixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }

        public BlockExpressionSyntax(
            TextSpan span,
            FixedList<IStatementSyntax> statements)
            : base(span)
        {
            Statements = statements;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            if (Statements.Any())
                return $"{{ {Statements.Count} Statements }} : {Type}";

            return $"{{ }} : {Type}";
        }
    }
}
