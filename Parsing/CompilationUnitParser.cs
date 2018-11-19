using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class CompilationUnitParser : ParserBase
    {
        [NotNull] private readonly IDeclarationParser declarationParser;
        [NotNull] private readonly IUsingDirectiveParser usingDirectiveParser;

        public CompilationUnitParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] IDeclarationParser declarationParser,
            [NotNull] IUsingDirectiveParser usingDirectiveParser)
            : base(tokens)
        {
            this.declarationParser = declarationParser;
            this.usingDirectiveParser = usingDirectiveParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives();
            var declarations = declarationParser.ParseDeclarations();
            Tokens.Required<IEndOfFileToken>();

            return new CompilationUnitSyntax(
                Tokens.Context.File,
                ParseImplicitNamespaceName(),
                usingDirectives,
                declarations,
                Tokens.Context.Diagnostics.Build());
        }

        private RootName ParseImplicitNamespaceName()
        {
            RootName name = GlobalNamespaceName.Instance;
            foreach (var segment in Tokens.Context.File.Reference.Namespace)
                name = name.Qualify(new SimpleName(segment));

            return name;
        }
    }
}
