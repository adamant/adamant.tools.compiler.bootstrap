using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class CompilationUnitParser
    {
        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ITokenIterator tokens)
        {
            var implicitNamespaceName = ParseImplicitNamespaceName(tokens);
            var parser = new Parser(tokens, implicitNamespaceName);
            var usingDirectives = parser.ParseUsingDirectives();
            var declarations = parser.ParseTopLevelDeclarations();
            tokens.Required<IEndOfFileToken>();

            return new CompilationUnitSyntax(tokens.Context.File,
                implicitNamespaceName, usingDirectives, declarations,
                tokens.Context.Diagnostics.Build());
        }

        [NotNull]
        private static RootName ParseImplicitNamespaceName([NotNull] ITokenIterator tokens)
        {
            RootName name = GlobalNamespaceName.Instance;
            foreach (var segment in tokens.Context.File.Reference.Namespace)
                name = name.Qualify(segment);

            return name;
        }
    }
}
