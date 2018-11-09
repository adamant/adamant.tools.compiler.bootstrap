using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    /// <summary>
    /// Doesn't implement IParser{CompilationUnitSyntax} because it doesn't take
    /// an <see cref="IDiagnosticsCollector"/>
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
        public CompilationUnitSyntax ParseCompilationUnit([NotNull] ITokenStream tokens)
        {
            var diagnostics = new DiagnosticsBuilder();
            var @namespace = ParseFileNamespaceDeclaration(tokens, diagnostics);
            var endOfFile = tokens.TakeEndOfFile();

            diagnostics.Publish(endOfFile.Diagnostics);

            return new CompilationUnitSyntax(tokens.File, @namespace, endOfFile, diagnostics.Build());
        }

        [MustUseReturnValue]
        [NotNull]
        private FileNamespaceDeclarationSyntax ParseFileNamespaceDeclaration(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var namespaceKeyword = tokens.Accept<NamespaceKeywordToken>();
            NameSyntax name = null;
            ISemicolonToken semicolon = null;
            if (namespaceKeyword != null)
            {
                name = nameParser.ParseName(tokens, diagnostics);
                semicolon = tokens.Expect<ISemicolonToken>();
            }
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives(tokens, diagnostics).ToSyntaxList();
            var declarations = declarationParser.ParseDeclarations(tokens, diagnostics).ToSyntaxList();
            return new FileNamespaceDeclarationSyntax(namespaceKeyword, name, semicolon, usingDirectives, declarations);
        }
    }
}
