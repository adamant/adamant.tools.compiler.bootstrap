using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Token
    {
        public readonly SourceText Source;
        public readonly int Start;
        public readonly int Length;
        public readonly TokenKind Kind;
        public string Text => Source.Content.Substring(Start, Length);
        public readonly bool IsMissing;
        public IEnumerable<Diagnostic> Diagnostics => diagnostics;
        private readonly List<Diagnostic> diagnostics;

        private Token(SourceText source, int start, int length, TokenKind kind, bool isMissing, IEnumerable<Diagnostic> diagnostics)
        {
            Requires.InString(source.Content, nameof(start), start, nameof(length), length);
            Requires.ValidEnum(nameof(kind), kind);
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Start = start;
            Length = length;
            Kind = kind;
            IsMissing = isMissing;
            this.diagnostics = (diagnostics ?? throw new ArgumentNullException(nameof(diagnostics))).ToList();
        }

        public static Token New(SourceText source, int start, int length, TokenKind kind, IEnumerable<Diagnostic> diagnostics)
        {
            return new Token(source, start, length, kind, false, diagnostics);
        }

        public static Token NewMissing(SourceText source, int start, TokenKind kind)
        {
            var diagnostic = new Diagnostic();
            return new Token(source, start, 0, kind, true, diagnostic.Yield());
        }
    }
}
