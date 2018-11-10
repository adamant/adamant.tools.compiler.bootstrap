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
        [CanBeNull] public BlockTerminator Terminator { get; private set; }

        public BasicBlock(int number)
        {
            Number = number;
            Statements = statements.AsReadOnly().NotNull();
        }

        public void Add([NotNull] Statement statement)
        {
            Requires.NotNull(nameof(statement), statement);
            statements.Add(statement);
        }

        public void End([NotNull] BlockTerminator terminator)
        {
            Requires.NotNull(nameof(terminator), terminator);
            // Can only set an terminator if there isn't already one
            Requires.Null(nameof(Terminator), Terminator);
            Terminator = terminator;
        }
    }
}
