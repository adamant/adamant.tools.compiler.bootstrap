using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    public class BasicBlock
    {
        public readonly int Number; // The block number is used as its name in IR
        public IReadOnlyList<Statement> Statements { get; }
        private readonly List<Statement> statements;
        public BranchingStatement TerminatingStatement { get; }

        public BasicBlock(
            IEnumerable<Statement> statements,
            BranchingStatement terminatingStatement,
            int number)
        {
            this.statements = statements.ToList();
            Statements = this.statements.AsReadOnly();
            TerminatingStatement = terminatingStatement;
            Number = number;
        }
    }
}
