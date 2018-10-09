using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class EndOfFileToken : Token
    {
        public IReadOnlyList<Diagnostic> Diagnostics { get; }

        public EndOfFileToken(TextSpan span, [NotNull] IEnumerable<Diagnostic> diagnostics)
            : base(span)
        {
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Diagnostics = diagnostics.ToList().AsReadOnly();
        }
    }
}
