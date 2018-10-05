using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public readonly struct EndOfFileToken : IToken
    {
        public TokenKind Kind => TokenKind.EndOfFile;
        public bool IsMissing => false;
        public readonly TextSpan Span;
        public readonly IReadOnlyList<Diagnostic> Value;

        public EndOfFileToken(TextSpan span, IEnumerable<Diagnostic> diagnostics)
        {
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Span = span;
            Value = diagnostics.ToList().AsReadOnly();
        }

        TextSpan IToken.Span => Span;
        object IToken.Value => Value;

        public static explicit operator EndOfFileToken(Token token)
        {
            return new EndOfFileToken(token.Span, (IReadOnlyList<Diagnostic>)token.Value);
        }
    }
}
