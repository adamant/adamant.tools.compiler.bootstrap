using System.Collections.Generic;
using System.Threading.Tasks;
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
            var expressionParserSource = new TaskCompletionSource<IExpressionParser>();
            var nameParser = new QualifiedNameParser(expressionParserSource.Task);
            var listParser = new ListParser();
            var usingDirectiveParser = new UsingDirectiveParser(nameParser);
            StatementParser statementParser = null;
            var expressionParser = new ExpressionParser(listParser, nameParser, () => statementParser);
            expressionParserSource.SetResult(expressionParser);
            var parameterParser = new ParameterParser(expressionParser);
            statementParser = new StatementParser(listParser, expressionParser);
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
