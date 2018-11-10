using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    /// <summary>
    /// Doesn't implement IParser{CompilationUnitSyntax} because it doesn't take
    /// an <see cref="Diagnostics"/>
    /// </summary>
    public class CompilationUnitParser
    {
        [NotNull] private readonly IUsingDirectiveParser usingDirectiveParser;
        [NotNull] private readonly IDeclarationParser declarationParser;
        [NotNull] private readonly INameParser nameParser;

        public CompilationUnitParser(
            [NotNull] IUsingDirectiveParser usingDirectiveParser,
            [NotNull] IDeclarationParser declarationParser,
            [NotNull] INameParser nameParser)
        {
            this.declarationParser = declarationParser;
            this.nameParser = nameParser;
            this.usingDirectiveParser = usingDirectiveParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public CompilationUnitSyntax ParseCompilationUnit([NotNull] ITokenIterator tokens)
        {
            var diagnostics = tokens.Context.Diagnostics;
            var @namespace = ParseFileNamespaceDeclaration(tokens, diagnostics);
            tokens.Consume<IEndOfFileToken>();

            return new CompilationUnitSyntax(tokens.Context.File, @namespace, diagnostics.Build());
        }

        [MustUseReturnValue]
        [NotNull]
        private FileNamespaceDeclarationSyntax ParseFileNamespaceDeclaration(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var namespaceKeyword = tokens.Accept<INamespaceKeywordToken>();
            NameSyntax name = null;
            ISemicolonTokenPlace semicolon = null;
            if (namespaceKeyword != null)
            {
                name = nameParser.ParseName(tokens, diagnostics);
                semicolon = tokens.Expect<ISemicolonTokenPlace>();
            }
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives(tokens, diagnostics).ToSyntaxList();
            var declarations = declarationParser.ParseDeclarations(tokens, diagnostics).ToSyntaxList();
            return new FileNamespaceDeclarationSyntax(namespaceKeyword, name, semicolon, usingDirectives, declarations);
        }
    }
}
