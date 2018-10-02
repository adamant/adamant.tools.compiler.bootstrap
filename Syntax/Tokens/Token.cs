using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class Token : SyntaxNode
    {
        public override CodeFile File { get; }
        public override TextSpan Span { get; }
        public readonly TokenKind Kind;
        public string Text => File.Code[Span];
        public readonly bool IsMissing;
        public IEnumerable<DiagnosticInfo> DiagnosticInfos => diagnosticInfos;
        private readonly List<DiagnosticInfo> diagnosticInfos;

        protected Token(CodeFile file, TextSpan span, TokenKind kind, bool isMissing, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            Requires.NotNull(nameof(file), file);
            Requires.InString(file.Code.Text, nameof(span), span);
            Requires.ValidEnum(nameof(kind), kind);
            File = file;
            Span = span;
            Kind = kind;
            IsMissing = isMissing;
            this.diagnosticInfos = (diagnosticInfos ?? throw new ArgumentNullException(nameof(diagnosticInfos))).ToList();
        }

        public static Token New(CodeFile file, TextSpan span, TokenKind kind, IEnumerable<DiagnosticInfo> diagnosticInfos)
        {
            Requires.That(nameof(kind), kind != TokenKind.StringLiteral);
            Requires.That(nameof(kind), kind != TokenKind.Identifier);
            return new Token(file, span, kind, false, diagnosticInfos);
        }

        public static Token Missing(CodeFile file, int start, TokenKind kind)
        {
            var diagnostic = Error.MissingToken(kind);
            var span = new TextSpan(start, 0);
            switch (kind)
            {
                case TokenKind.Identifier:
                    return new IdentifierToken(file, span, IdentifierKind.Normal, true, "", diagnostic.Yield());
                case TokenKind.StringLiteral:
                    return new StringLiteralToken(file, span, true, "", diagnostic.Yield());
                default:
                    return new Token(file, span, kind, true, diagnostic.Yield());
            }
        }

        public override string ToString()
        {
            var validMarker = IsMissing || DiagnosticInfos.Any(d => d.Level > DiagnosticLevel.Warning) ? "*" : "";
            return $"{Kind}{validMarker}=`{Text}`";
        }

        public override void AllDiagnostics(IList<Diagnostic> list)
        {
            foreach (var diagnosticInfo in diagnosticInfos)
                list.Add(new Diagnostic(File, Span, diagnosticInfo));
        }
    }
}
