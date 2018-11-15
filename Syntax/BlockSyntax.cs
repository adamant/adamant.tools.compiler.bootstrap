using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
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
            return $"{{{Statements}}}";
        }
    }
}
