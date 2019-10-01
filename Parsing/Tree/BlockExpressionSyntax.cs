using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BlockExpressionSyntax : ExpressionSyntax, IBlockExpressionSyntax
    {
        public FixedList<IStatementSyntax> Statements { get; }

        public BlockExpressionSyntax(
            TextSpan span,
            FixedList<IStatementSyntax> statements)
            : base(span)
        {
            Statements = statements;
        }

        public override string ToString()
        {
            if (Statements.Any())
                return $"{{ {Statements.Count} Statements }} : {Type}";

            return $"{{ }} : {Type}";
        }
    }
}
