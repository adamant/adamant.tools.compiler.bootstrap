using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class FileParser
    {
        [NotNull] private readonly CompilationUnitParser compilationUnitParser;

        public FileParser([NotNull] ITokenIterator tokens)
        {
            var listParser = new ListParser();
            var functionBodyParser = new FunctionBodyParser(listParser);
            var genericsParser = new GenericsParser(listParser, functionBodyParser);
            var usingDirectiveParser = new UsingDirectiveParser(functionBodyParser);
            var parameterParser = new ParameterParser(tokens, functionBodyParser);
            var accessModifierParser = new ModifierParser();
            var declarationParser = new DeclarationParser(listParser, functionBodyParser,
                functionBodyParser, parameterParser, accessModifierParser,
                genericsParser, functionBodyParser, usingDirectiveParser);
            compilationUnitParser = new CompilationUnitParser(usingDirectiveParser, declarationParser, functionBodyParser);
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ParseContext context, [NotNull]  IEnumerable<IToken> tokens)
        {
            return Parse(new TokenIterator(context, tokens));
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ITokenIterator tokens)
        {
            return compilationUnitParser.ParseCompilationUnit(tokens);
        }
    }
}
