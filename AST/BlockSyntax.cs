using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// A block in a statement or expression. Not to be used to represent function
    /// or type bodies.
    /// </summary>
    /// <remarks>This breaks with the convention of giving expression classes names
    /// ending in "ExpressionSyntax". This is because a block can be either an
    /// expression or can be a block for a control flow statement. In the latter
    /// it is acting more like a statement.</remarks>
    public class BlockSyntax : ExpressionSyntax, IBlockOrResultSyntax
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
