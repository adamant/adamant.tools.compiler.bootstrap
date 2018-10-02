using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements
{
    public class Block : Statement
    {
        public new BlockSyntax Syntax { get; }
        public IReadOnlyList<Statement> Statements { get; }

        public Block(
            BlockSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            IEnumerable<Statement> statements)
            : base(diagnostics)
        {
            Syntax = syntax;
            Statements = statements.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax() => Syntax;

        public override void AllDiagnostics(IList<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            foreach (var statement in Statements)
                statement.AllDiagnostics(list);
        }
    }
}
