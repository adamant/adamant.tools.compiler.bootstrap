using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class FileParser
    {
        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ITokenIterator tokens)
        {
            var listParser = new ListParser(tokens);
            var parser = new Parser(tokens, listParser);
            var genericsParser = new GenericsParser(tokens, listParser, parser);
            var usingDirectiveParser = new UsingDirectiveParser(tokens, parser, listParser);
            var parameterParser = new ParameterParser(tokens, parser);
            var accessModifierParser = new ModifierParser(tokens);
            var declarationParser = new DeclarationParser(tokens, listParser, parser,
                parser, parameterParser, accessModifierParser,
                genericsParser, parser, usingDirectiveParser,
                GlobalNamespaceName.Instance);
            var compilationUnitParser = new CompilationUnitParser(tokens, declarationParser, usingDirectiveParser);
            return compilationUnitParser.ParseCompilationUnit();
        }
    }
}
