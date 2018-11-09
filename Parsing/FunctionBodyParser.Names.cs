using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser : INameParser
    {
        [MustUseReturnValue]
        [NotNull]
        public NameSyntax ParseName([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            NameSyntax name = ParseSimpleName(tokens, diagnostics);
            while (tokens.Current is DotToken)
            {
                var dot = tokens.Take<DotToken>();
                var simpleName = ParseSimpleName(tokens, diagnostics);
                name = new QualifiedNameSyntax(name, dot, simpleName);
            }
            return name;
        }

        [MustUseReturnValue]
        [NotNull]
        private SimpleNameSyntax ParseSimpleName([NotNull]ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            var identifier = tokens.ExpectIdentifier();
            SimpleNameSyntax simpleName;
            if (tokens.Current is OpenBracketToken)
            {
                var openBracket = tokens.Expect<IOpenBracketToken>();
                var arguments = ParseArgumentList(tokens, diagnostics);
                var closeBracket = tokens.Expect<ICloseBracketToken>();
                simpleName = new GenericNameSyntax(identifier, openBracket, arguments, closeBracket);
            }
            else
                simpleName = new IdentifierNameSyntax(identifier);
            return simpleName;
        }
    }
}
