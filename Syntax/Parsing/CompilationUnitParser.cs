using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
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
        [NotNull] private readonly IParser<UsingDirectiveSyntax> usingDirectiveParser;
        [NotNull] private readonly IParser<DeclarationSyntax> declarationParser;
        [NotNull] private readonly IParser<NameSyntax> qualifiedNameParser;

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
            var diagnosticsBuilder = new DiagnosticsBuilder();
            var @namespace = ParseCompilationUnitNamespace(tokens, diagnosticsBuilder);
            var usingDirectives = ParseUsingDirectives(tokens, diagnosticsBuilder).ToSyntaxList();
            var declarations = ParseDeclarations(tokens, diagnosticsBuilder).ToSyntaxList();
            var endOfFile = tokens.TakeEndOfFile().AssertNotNull();

            diagnosticsBuilder.Publish(endOfFile.Diagnostics);

            return new CompilationUnitSyntax(tokens.File, @namespace, usingDirectives, declarations, endOfFile, diagnosticsBuilder.Build());
        }

        [MustUseReturnValue]
        [CanBeNull]
        private CompilationUnitNamespaceSyntax ParseCompilationUnitNamespace([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            if (!(tokens.Current is NamespaceKeywordToken)) return null;

            var namespaceKeyword = tokens.Expect<INamespaceKeywordToken>();
            var name = qualifiedNameParser.Parse(tokens, diagnostics);
            var semicolon = tokens.Expect<ISemicolonToken>();
            return new CompilationUnitNamespaceSyntax(namespaceKeyword, name, semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private IEnumerable<UsingDirectiveSyntax> ParseUsingDirectives([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            while (tokens.Current is UsingKeywordToken)
            {
                yield return usingDirectiveParser.Parse(tokens, diagnostics);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private IEnumerable<DeclarationSyntax> ParseDeclarations([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            while (!tokens.AtEndOfFile())
            {
                yield return declarationParser.Parse(tokens, diagnostics);
            }
        }
    }
}
