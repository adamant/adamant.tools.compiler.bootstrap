using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
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
        [NotNull] private readonly INameParser qualifiedNameParser;

        public CompilationUnitParser(
            [NotNull] IUsingDirectiveParser usingDirectiveParser,
            [NotNull] IDeclarationParser declarationParser,
            [NotNull] INameParser qualifiedNameParser)
        {
            this.declarationParser = declarationParser;
            this.qualifiedNameParser = qualifiedNameParser;
            this.usingDirectiveParser = usingDirectiveParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public CompilationUnitSyntax ParseCompilationUnit([NotNull] ITokenStream tokens)
        {
            var diagnosticsBuilder = new DiagnosticsBuilder();
            var @namespace = AcceptCompilationUnitNamespace(tokens, diagnosticsBuilder);
            var usingDirectives = ParseUsingDirectives(tokens, diagnosticsBuilder).ToSyntaxList();
            var declarations = ParseDeclarations(tokens, diagnosticsBuilder).ToSyntaxList();
            var endOfFile = tokens.TakeEndOfFile();

            diagnosticsBuilder.Publish(endOfFile.Diagnostics);

            return new CompilationUnitSyntax(tokens.File, @namespace, usingDirectives, declarations, endOfFile, diagnosticsBuilder.Build());
        }

        [MustUseReturnValue]
        [CanBeNull]
        private CompilationUnitNamespaceSyntax AcceptCompilationUnitNamespace([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            if (!(tokens.Current is NamespaceKeywordToken)) return null;

            var namespaceKeyword = tokens.Expect<INamespaceKeywordToken>();
            var name = qualifiedNameParser.ParseName(tokens, diagnostics);
            var semicolon = tokens.Expect<ISemicolonToken>();
            return new CompilationUnitNamespaceSyntax(namespaceKeyword, name, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private IEnumerable<UsingDirectiveSyntax> ParseUsingDirectives([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            while (tokens.Current is UsingKeywordToken)
            {
                yield return usingDirectiveParser.ParseUsingDirective(tokens, diagnostics);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private IEnumerable<DeclarationSyntax> ParseDeclarations([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            while (!tokens.AtEndOfFile())
            {
                yield return declarationParser.ParseDeclaration(tokens, diagnostics);
            }
        }
    }
}
