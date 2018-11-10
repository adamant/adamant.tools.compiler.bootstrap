using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class FileParser
    {
        [NotNull] private readonly CompilationUnitParser compilationUnitParser;

        public FileParser([NotNull] ITokenIterator tokens)
        {
            var listParser = new ListParser(tokens);
            var functionBodyParser = new FunctionBodyParser(listParser);
            var genericsParser = new GenericsParser(listParser, functionBodyParser);
            var usingDirectiveParser = new UsingDirectiveParser(functionBodyParser);
            var parameterParser = new ParameterParser(tokens, functionBodyParser);
            var accessModifierParser = new ModifierParser(tokens);
            var declarationParser = new DeclarationParser(tokens, listParser, functionBodyParser,
                functionBodyParser, parameterParser, accessModifierParser,
                genericsParser, functionBodyParser, usingDirectiveParser);
            compilationUnitParser = new CompilationUnitParser(tokens, declarationParser);
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse()
        {
            return compilationUnitParser.ParseCompilationUnit();
        }
    }
}
