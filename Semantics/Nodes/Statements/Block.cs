using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements
{
    public class Block : Statement
    {
        [NotNull] public new BlockStatementSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Statement> Statements { get; }

        public Block(
            [NotNull] BlockStatementSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [NotNull] [ItemNotNull] IEnumerable<Statement> statements)
            : base(diagnostics)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(statements), statements);
            Syntax = syntax;
            Statements = statements.ToList().AsReadOnly().AssertNotNull();
        }

        [NotNull]
        protected override SyntaxNode GetSyntax() => Syntax;

        public override void AllDiagnostics([NotNull][ItemNotNull] List<Diagnostic> list)
        {
            Requires.NotNull(nameof(list), list);
            base.AllDiagnostics(list);
            foreach (var statement in Statements)
                statement.AllDiagnostics(list);
        }
    }
}
