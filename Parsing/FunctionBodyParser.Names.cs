using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser : INameParser
    {
        [MustUseReturnValue]
        [NotNull]
        public NameSyntax ParseName([NotNull] ITokenIterator tokens, [NotNull] Diagnostics diagnostics)
        {
            NameSyntax name = ParseSimpleName(tokens, diagnostics);
            while (tokens.Current is IDotToken)
            {
                var dot = tokens.Take<IDotToken>();
                var simpleName = ParseSimpleName(tokens, diagnostics);
                name = new QualifiedNameSyntax(name, dot, simpleName);
            }
            return name;
        }

        [MustUseReturnValue]
        [NotNull]
        private SimpleNameSyntax ParseSimpleName([NotNull]ITokenIterator tokens, [NotNull] Diagnostics diagnostics)
        {
            var identifier = tokens.ExpectIdentifier();
            SimpleNameSyntax simpleName;
            if (tokens.Current is IOpenBracketToken)
            {
                var openBracket = tokens.Expect<IOpenBracketTokenPlace>();
                var arguments = ParseArgumentList(tokens, diagnostics);
                var closeBracket = tokens.Expect<ICloseBracketTokenPlace>();
                simpleName = new GenericNameSyntax(identifier, openBracket, arguments, closeBracket);
            }
            else
                simpleName = new IdentifierNameSyntax(identifier);
            return simpleName;
        }
    }
}
