using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class Body : AbstractSyntax, IBody
    {
        public FixedList<IBodyStatement> Statements { get; }
        FixedList<IStatement> IBodyOrBlock.Statements => Statements.ToFixedList<IStatement>();

        public Body(TextSpan span, FixedList<IBodyStatement> statements)
            : base(span)
        {
            Statements = statements;
        }
    }
}
