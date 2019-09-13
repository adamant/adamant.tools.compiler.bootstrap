using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class BlockSyntax : ExpressionBlockSyntax
    {
        public FixedList<StatementSyntax> Statements { get; }

        public BlockSyntax(
            TextSpan span,
            FixedList<StatementSyntax> statements)
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
