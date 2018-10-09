using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SkippedTokensSyntax : SyntaxNode
    {
        // TODO remove
        public IReadOnlyList<Diagnostic> Diagnostics;

        public SkippedTokensSyntax(CodeFile file, Token token)
        {
            var diagnostics = new List<Diagnostic>();
            if (token is UnexpectedToken)
                diagnostics.Add(SyntaxError.SkippedToken(file, token.Span));

            Diagnostics = diagnostics.AsReadOnly();
        }
    }
}
