using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    /// <summary>
    /// A block in a statement or expression. Not to be used to represent function
    /// or type bodies.
    /// </summary>
    /// <remarks>This breaks with the convention of giving expression classes names
    /// ending in "ExpressionSyntax". This is because a block can be either an
    /// expression or can be a block for a control flow statement. In the latter
    /// it is acting more like a statement.</remarks>
    internal class BlockSyntax : ExpressionSyntax, IBlockSyntax
    {
        public FixedList<IStatementSyntax> Statements { get; }

        public BlockSyntax(
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
