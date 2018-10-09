using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Parser : IParser<CompilationUnitSyntax>
    {
        private readonly IParser<CompilationUnitSyntax> compilationUnitParser;

        public Parser()
        {
            var nameParser = new QualifiedNameParser();
            var listParser = new ListParser();
            var usingDirectiveParser = new UsingDirectiveParser(nameParser);
            var expressionParser = new ExpressionParser(listParser, nameParser);
            var parameterParser = new ParameterParser(expressionParser);
            var statementParser = new StatementParser(listParser, expressionParser);
            var declarationParser = new DeclarationParser(listParser, expressionParser, statementParser, parameterParser);
            compilationUnitParser = new CompilationUnitParser(usingDirectiveParser, declarationParser, nameParser);
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse(CodeFile file, IEnumerable<Token> tokens)
        {
            return Parse(new TokenStream(file, tokens));
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse(ITokenStream tokens)
        {
            return compilationUnitParser.Parse(tokens);
        }
    }
}
