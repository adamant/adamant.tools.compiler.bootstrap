using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
            // TODO actually pass in the correct implicit namespace name
            var @namespace = declarationParser.ParseFileNamespace(FixedList<string>.Empty);
            Tokens.Required<IEndOfFileToken>();

            return new CompilationUnitSyntax(Tokens.Context.File, @namespace, Tokens.Context.Diagnostics.Build());
        }
    }
}
