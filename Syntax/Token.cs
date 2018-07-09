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
        public IEnumerable<DiagnosticInfo> DiagnosticInfos => diagnosticInfos;
        private readonly List<DiagnosticInfo> diagnosticInfos;

        private Token(SourceText source, int start, int length, TokenKind kind, bool isMissing, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            Requires.InString(source.Content, nameof(start), start, nameof(length), length);
            Requires.ValidEnum(nameof(kind), kind);
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Start = start;
            Length = length;
            Kind = kind;
            IsMissing = isMissing;
            this.diagnosticInfos = (diagnosticInfos ?? throw new ArgumentNullException(nameof(diagnosticInfos))).ToList();
        }

        public static Token New(SourceText source, int start, int length, TokenKind kind, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            return new Token(source, start, length, kind, false, diagnosticInfos);
        }

        public static Token NewMissing(SourceText source, int start, TokenKind kind)
        {
            var diagnostic = new DiagnosticInfo(DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, $"Missing token of kind {kind}");
            return new Token(source, start, 0, kind, true, diagnostic.Yield());
        }
    }
}
