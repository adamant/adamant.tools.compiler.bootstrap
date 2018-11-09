using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class EndOfFileToken : Token, IEndOfFileToken
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

    public static partial class TokenFactory
    {
        [NotNull]
        public static IEndOfFileToken EndOfFile(TextSpan span, [NotNull] Diagnostics diagnostics)
        {
            return new EndOfFileToken(span, diagnostics);
        }
    }
}
