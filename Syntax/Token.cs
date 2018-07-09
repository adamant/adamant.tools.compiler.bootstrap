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
        public readonly TextSpan Span;
        public readonly TokenKind Kind;
        public string Text => Source[Span];
        public readonly bool IsMissing;
        public IEnumerable<DiagnosticInfo> DiagnosticInfos => diagnosticInfos;
        private readonly List<DiagnosticInfo> diagnosticInfos;

        private Token(SourceText source, TextSpan span, TokenKind kind, bool isMissing, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            Requires.InString(source.Content, nameof(span), span);
            Requires.ValidEnum(nameof(kind), kind);
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Span = span;
            Kind = kind;
            IsMissing = isMissing;
            this.diagnosticInfos = (diagnosticInfos ?? throw new ArgumentNullException(nameof(diagnosticInfos))).ToList();
        }

        public static Token New(SourceText source, TextSpan span, TokenKind kind, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            return new Token(source, span, kind, false, diagnosticInfos);
        }

        public static Token NewMissing(SourceText source, int start, TokenKind kind)
        {
            var diagnostic = new DiagnosticInfo(DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, $"Missing token of kind {kind}");
            return new Token(source, new TextSpan(start, 0), kind, true, diagnostic.Yield());
        }
    }
}
