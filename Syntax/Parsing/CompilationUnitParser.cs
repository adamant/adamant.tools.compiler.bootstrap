using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class CompilationUnitParser : IParser<CompilationUnitSyntax>
    {
        private readonly IParser<UsingDirectiveSyntax> usingDirectiveParser;
        private readonly IParser<DeclarationSyntax> declarationParser;
        private readonly IParser<NameSyntax> qualifiedNameParser;

        public CompilationUnitParser(
            IParser<UsingDirectiveSyntax> usingDirectiveParser,
            IParser<DeclarationSyntax> declarationParser,
            IParser<NameSyntax> qualifiedNameParser)
        {
            this.declarationParser = declarationParser;
            this.qualifiedNameParser = qualifiedNameParser;
            this.usingDirectiveParser = usingDirectiveParser;
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse(ITokenStream tokens)
        {
            var @namespace = ParseCompilationUnitNamespace(tokens);
            var usingDirectives = ParseUsingDirectives(tokens).ToSyntaxList();
            var declarations = ParseDeclarations(tokens).ToSyntaxList();
            var endOfFile = tokens.ExpectEndOfFile();

            return new CompilationUnitSyntax(@namespace, usingDirectives, declarations, endOfFile);
        }

        [MustUseReturnValue]
        private CompilationUnitNamespaceSyntax ParseCompilationUnitNamespace(ITokenStream tokens)
        {
            if (!tokens.CurrentIs(TokenKind.NamespaceKeyword)) return null;

            var namespaceKeyword = tokens.ExpectSimple(TokenKind.NamespaceKeyword);
            var name = qualifiedNameParser.Parse(tokens);
            var semicolon = tokens.ExpectSimple(TokenKind.Semicolon);
            return new CompilationUnitNamespaceSyntax(namespaceKeyword, name, semicolon);
        }

        [MustUseReturnValue]
        private IEnumerable<UsingDirectiveSyntax> ParseUsingDirectives(ITokenStream tokens)
        {
            while (tokens.CurrentIs(TokenKind.UsingKeyword))
            {
                yield return usingDirectiveParser.Parse(tokens);
            }
        }

        [MustUseReturnValue]
        private IEnumerable<DeclarationSyntax> ParseDeclarations(ITokenStream tokens)
        {
            while (!tokens.AtEndOfFile())
            {
                yield return declarationParser.Parse(tokens);
            }
        }
    }
}
