using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SkippedTokensSyntax : SyntaxBranchNode
    {
        public IReadOnlyList<DiagnosticInfo> Diagnostics;

        public SkippedTokensSyntax(Token token)
            : base(token.Yield())
        {
            var diagnostics = new List<DiagnosticInfo>();
            if (token.Kind != TokenKind.Unexpected)
                diagnostics.Add(Error.SkippedToken());

            Diagnostics = diagnostics.AsReadOnly();
        }

        public override void AllDiagnostics(IList<Diagnostic> diagnostics)
        {
            base.AllDiagnostics(diagnostics);
            foreach (var diagnosticInfo in Diagnostics)
            {
                diagnostics.Add(new Diagnostic(File, Span, diagnosticInfo));
            }
        }
    }
}
