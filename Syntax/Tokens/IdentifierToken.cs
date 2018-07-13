using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class IdentifierToken : Token
    {
        public readonly IdentifierKind IdentifierKind;
        public readonly string Value;

        public IdentifierToken(
            SourceText source,
            TextSpan span,
            IdentifierKind kind,
            string value,
            IEnumerable<DiagnosticInfo> diagnosticInfos)
            : base(source, span, TokenKind.Identifier, false, diagnosticInfos)
        {
            IdentifierKind = kind;
            Value = value;
        }
    }
}