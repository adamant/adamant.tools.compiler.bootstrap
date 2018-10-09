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
        [NotNull] private readonly IParser<CompilationUnitSyntax> compilationUnitParser;

        public Parser()
        {
            var nameParser = new QualifiedNameParser();
            var listParser = new ListParser();
            var usingDirectiveParser = new UsingDirectiveParser(nameParser);
            var expressionParser = new ExpressionParser(listParser, nameParser);
            var parameterParser = new ParameterParser(expressionParser);
            var statementParser = new StatementParser(listParser, expressionParser);
            var accessModifierParser = new AccessModifierParser();
            var declarationParser = new DeclarationParser(listParser, expressionParser, statementParser, parameterParser, accessModifierParser);
            compilationUnitParser = new CompilationUnitParser(usingDirectiveParser, declarationParser, nameParser);
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] CodeFile file, [NotNull]  IEnumerable<Token> tokens)
        {
            return Parse(new TokenStream(file, tokens));
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ITokenStream tokens)
        {
            return compilationUnitParser.Parse(tokens);
        }
    }
}
