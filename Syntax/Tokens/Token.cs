using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class Token : SyntaxNode
    {
        public readonly CodeText Code;
        public readonly TextSpan Span;
        public readonly TokenKind Kind;
        public string Text => Code[Span];
        public readonly bool IsMissing;
        public IEnumerable<DiagnosticInfo> DiagnosticInfos => diagnosticInfos;
        private readonly List<DiagnosticInfo> diagnosticInfos;

        protected Token(CodeText code, TextSpan span, TokenKind kind, bool isMissing, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            Requires.InString(code.Text, nameof(span), span);
            Requires.ValidEnum(nameof(kind), kind);
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Span = span;
            Kind = kind;
            IsMissing = isMissing;
            this.diagnosticInfos = (diagnosticInfos ?? throw new ArgumentNullException(nameof(diagnosticInfos))).ToList();
        }

        public static Token New(CodeText code, TextSpan span, TokenKind kind, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            Requires.That(nameof(kind), kind != TokenKind.StringLiteral);
            Requires.That(nameof(kind), kind != TokenKind.Identifier);
            return new Token(code, span, kind, false, diagnosticInfos);
        }

        public static Token Missing(CodeText code, int start, TokenKind kind)
        {
            Requires.That(nameof(kind), kind != TokenKind.StringLiteral);
            Requires.That(nameof(kind), kind != TokenKind.Identifier);
            var diagnostic = new DiagnosticInfo(DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, $"Missing token of kind {kind}");
            return new Token(code, new TextSpan(start, 0), kind, true, diagnostic.Yield());
        }

        public override string ToString()
        {
            var validMarker = IsMissing || DiagnosticInfos.Any(d => d.Level > DiagnosticLevel.Warning) ? "*" : "";
            return $"{Kind}{validMarker}=`{Text}`";
        }
    }
}
