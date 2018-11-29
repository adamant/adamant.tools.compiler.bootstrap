using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class BlockSyntax : ExpressionBlockSyntax
    {
        [NotNull, ItemNotNull] public FixedList<StatementSyntax> Statements { get; }

        public BlockSyntax(
            TextSpan span,
            [NotNull, ItemNotNull] FixedList<StatementSyntax> statements)
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
