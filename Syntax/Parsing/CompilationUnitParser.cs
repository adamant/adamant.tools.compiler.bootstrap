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
        [NotNull]
        private readonly IParser<UsingDirectiveSyntax> usingDirectiveParser;

        [NotNull]
        private readonly IParser<DeclarationSyntax> declarationParser;

        [NotNull]
        private readonly IParser<NameSyntax> qualifiedNameParser;

        public CompilationUnitParser(
            [NotNull] IParser<UsingDirectiveSyntax> usingDirectiveParser,
            [NotNull] IParser<DeclarationSyntax> declarationParser,
            [NotNull] IParser<NameSyntax> qualifiedNameParser)
        {
            this.declarationParser = declarationParser;
            this.qualifiedNameParser = qualifiedNameParser;
            this.usingDirectiveParser = usingDirectiveParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public CompilationUnitSyntax Parse([NotNull] ITokenStream tokens)
        {
            var @namespace = ParseCompilationUnitNamespace(tokens);
            var usingDirectives = ParseUsingDirectives(tokens).ToSyntaxList();
            var declarations = ParseDeclarations(tokens).ToSyntaxList();
            var endOfFile = tokens.ExpectEndOfFile();

            return new CompilationUnitSyntax(@namespace, usingDirectives, declarations, endOfFile);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private CompilationUnitNamespaceSyntax ParseCompilationUnitNamespace([NotNull] ITokenStream tokens)
        {
            if (!(tokens.Current is NamespaceKeywordToken)) return null;

            var namespaceKeyword = tokens.Expect<NamespaceKeywordToken>();
            var name = qualifiedNameParser.Parse(tokens);
            var semicolon = tokens.Expect<SemicolonToken>();
            return new CompilationUnitNamespaceSyntax(namespaceKeyword, name, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private IEnumerable<UsingDirectiveSyntax> ParseUsingDirectives([NotNull] ITokenStream tokens)
        {
            while (tokens.Current is UsingKeywordToken)
            {
                yield return usingDirectiveParser.Parse(tokens);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private IEnumerable<DeclarationSyntax> ParseDeclarations([NotNull] ITokenStream tokens)
        {
            while (!tokens.AtEndOfFile())
            {
                yield return declarationParser.Parse(tokens);
            }
        }
    }
}
