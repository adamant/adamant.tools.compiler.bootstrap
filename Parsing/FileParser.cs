using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class FileParser
    {
        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ITokenIterator tokens)
        {
            var listParser = new ListParser(tokens);
            var functionBodyParser = new FunctionBodyParser(tokens, listParser);
            var genericsParser = new GenericsParser(tokens, listParser, functionBodyParser);
            var usingDirectiveParser = new UsingDirectiveParser(tokens, functionBodyParser, listParser);
            var parameterParser = new ParameterParser(tokens, functionBodyParser);
            var accessModifierParser = new ModifierParser(tokens);
            var declarationParser = new DeclarationParser(tokens, listParser, functionBodyParser,
                functionBodyParser, parameterParser, accessModifierParser,
                genericsParser, functionBodyParser, usingDirectiveParser);
            var compilationUnitParser = new CompilationUnitParser(tokens, declarationParser);
            return compilationUnitParser.ParseCompilationUnit();
        }
    }
}
