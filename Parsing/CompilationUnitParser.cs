using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.NotImplemented;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class CompilationUnitParser
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Entry point class which may need to be non-static in future")]
        public ICompilationUnitSyntax Parse(ITokenIterator<IEssentialToken> tokens)
        {
            var implicitNamespaceName = ParseImplicitNamespaceName(tokens);
            var parser = new Parser(tokens, implicitNamespaceName.ToRootName(), implicitNamespaceName);
            var usingDirectives = parser.ParseUsingDirectives();
            var declarations = parser.ParseNonMemberDeclarations<IEndOfFileToken>();
            var eof = tokens.Required<IEndOfFileToken>();
            var span = TextSpan.FromStartEnd(0, eof.End);
            var diagnostics = tokens.Context.Diagnostics;
            var compilationUnit = new CompilationUnitSyntax(implicitNamespaceName, span,
                tokens.Context.File, usingDirectives,
                declarations);

            CheckSyntax(compilationUnit, diagnostics);
            compilationUnit.Attach(diagnostics.Build());
            return compilationUnit;
        }

        private static NamespaceName ParseImplicitNamespaceName(ITokenIterator<IEssentialToken> tokens)
        {
            NamespaceName name = NamespaceName.Global;
            foreach (var segment in tokens.Context.File.Reference.Namespace)
                name = name.Qualify(segment);

            return name;
        }

        private static void CheckSyntax(CompilationUnitSyntax compilationUnit, Diagnostics diagnostics)
        {
            var notImplementedChecker = new SyntaxNotImplementedChecker(compilationUnit, diagnostics);
            notImplementedChecker.Check();
        }
    }
}
