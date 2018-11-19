using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class CompilationUnitParser : Parser
    {
        [NotNull] private readonly IDeclarationParser declarationParser;

        public CompilationUnitParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] IDeclarationParser declarationParser)
            : base(tokens)
        {
            this.declarationParser = declarationParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var @namespace = declarationParser.ParseFileNamespace(Tokens.Context.File.Reference.Namespace);
            Tokens.Required<IEndOfFileToken>();

            return new CompilationUnitSyntax(Tokens.Context.File, @namespace, Tokens.Context.Diagnostics.Build());
        }
    }
}
