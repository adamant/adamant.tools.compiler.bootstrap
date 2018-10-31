using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Parser
    {
        [NotNull] private readonly CompilationUnitParser compilationUnitParser;

        public Parser()
        {
            var listParser = new ListParser();
            var functionBodyParser = new FunctionBodyParser(listParser);
            var genericsParser = new GenericsParser(listParser, functionBodyParser);
            var usingDirectiveParser = new UsingDirectiveParser(functionBodyParser);
            var parameterParser = new ParameterParser(functionBodyParser);
            var accessModifierParser = new ModifierParser();
            var declarationParser = new DeclarationParser(listParser, functionBodyParser,
                functionBodyParser, parameterParser, accessModifierParser, genericsParser);
            compilationUnitParser = new CompilationUnitParser(usingDirectiveParser, declarationParser, functionBodyParser);
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] CodeFile file, [NotNull]  IEnumerable<Token> tokens)
        {
            return Parse(new TokenStream(file, tokens));
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ITokenStream tokens)
        {
            return compilationUnitParser.ParseCompilationUnit(tokens);
        }
    }
}
