using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code
{
    public class BasicBlock
    {
        public readonly int Number; // The block number is used as its name in IR
        public IReadOnlyList<Statement> Statements { get; }
        private readonly List<Statement> statements = new List<Statement>();
        public EndStatement EndStatement { get; private set; }

        public BasicBlock(int number)
        {
            Number = number;
            Statements = statements.AsReadOnly();
        }

        public void Add(Statement statement)
        {
            statements.Add(statement);
        }

        public void End(EndStatement endStatement)
        {
            Requires.That(nameof(EndStatement), EndStatement == null);
            EndStatement = endStatement;
        }

        internal void ToString(AsmBuilder builder)
        {
            builder.AppendLine($"bb{Number}:");
            builder.BeginBlock();
            foreach (var statement in statements)
                statement.ToString(builder);

            EndStatement.ToString(builder);
            builder.EndBlock();
        }
    }
}
