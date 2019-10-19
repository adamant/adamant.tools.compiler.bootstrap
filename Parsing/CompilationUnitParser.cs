using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class CompilationUnitParser
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Entry point to class which may need to be non-static in future")]
        public ICompilationUnitSyntax Parse(ITokenIterator<IEssentialToken> tokens)
        {
            var implicitNamespaceName = ParseImplicitNamespaceName(tokens);
            var parser = new Parser(tokens, implicitNamespaceName);
            var usingDirectives = parser.ParseUsingDirectives();
            var declarations = parser.ParseNonMemberDeclarations<IEndOfFileToken>();
            var eof = tokens.Required<IEndOfFileToken>();
            var span = TextSpan.FromStartEnd(0, eof.End);
            return new CompilationUnitSyntax(implicitNamespaceName, span,
                tokens.Context.File, usingDirectives,
                declarations, tokens.Context.Diagnostics.Build());
        }

        private static RootName ParseImplicitNamespaceName(ITokenIterator<IEssentialToken> tokens)
        {
            RootName name = GlobalNamespaceName.Instance;
            foreach (var segment in tokens.Context.File.Reference.Namespace)
                name = name.Qualify(segment);

            return name;
        }
    }
}
