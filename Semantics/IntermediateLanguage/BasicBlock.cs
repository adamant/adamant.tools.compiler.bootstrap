using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class BasicBlock
    {
        public readonly int Number; // The block number is used as its name in IR
        [NotNull] [ItemNotNull] public IReadOnlyList<Statement> Statements { get; }
        [NotNull] [ItemNotNull] private readonly List<Statement> statements = new List<Statement>();
        [CanBeNull] public EndStatement EndStatement { get; private set; }

        public BasicBlock(int number)
        {
            Number = number;
            Statements = statements.AsReadOnly().AssertNotNull();
        }

        public void Add([NotNull] SimpleStatement statement)
        {
            Requires.NotNull(nameof(statement), statement);
            if (EndStatement == null)
                statements.Add(statement);
            else
                statements.Insert(statements.Count - 2, statement);
        }

        public void End([NotNull] EndStatement endStatement)
        {
            Requires.NotNull(nameof(endStatement), endStatement);
            // Can only set an end statement if there isn't already one
            Requires.Null(nameof(EndStatement), EndStatement);
            EndStatement = endStatement;
            statements.Add(EndStatement);
        }
    }
}
