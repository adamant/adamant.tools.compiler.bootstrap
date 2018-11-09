using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class EndOfFileToken : Token
    {
        [NotNull]
        public Diagnostics Diagnostics { get; }

        public EndOfFileToken(TextSpan span, [NotNull] Diagnostics diagnostics)
            : base(span)
        {
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Diagnostics = diagnostics;
        }
    }
}
