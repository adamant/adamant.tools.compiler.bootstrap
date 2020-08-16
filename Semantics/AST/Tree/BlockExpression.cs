using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class BlockExpression : Expression, IBlockExpression
    {
        public FixedList<IStatement> Statements { get; }

        public BlockExpression(TextSpan span, DataType dataType, FixedList<IStatement> statements)
            : base(span, dataType)
        {
            Statements = statements;
        }
    }
}
