using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class ParameterParser : IParameterParser
    {
        [NotNull] private readonly IExpressionParser expressionParser;

        public ParameterParser([NotNull] IExpressionParser expressionParser)
        {
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public ParameterSyntax ParseParameter(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case IMutableKeywordToken mutableKeyword:
                {
                    tokens.MoveNext();
                    var selfKeyword = tokens.Expect<ISelfKeywordTokenPlace>();
                    return new SelfParameterSyntax(mutableKeyword, selfKeyword);
                }
                case ISelfKeywordToken selfKeyword:
                    tokens.MoveNext();
                    return new SelfParameterSyntax(null, selfKeyword);
                default:
                    var paramsKeyword = tokens.Accept<IParamsKeywordToken>();
                    var varKeyword = tokens.Accept<IVarKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var colon = tokens.Expect<IColonTokenPlace>();
                    // Need to not consume the assignment that separates the type from the default value,
                    // hence the min operator precedence.
                    var typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.AboveAssignment);
                    var equals = tokens.Accept<IEqualsToken>();
                    ExpressionSyntax defaultValue = null;
                    if (equals != null)
                        defaultValue = expressionParser.ParseExpression(tokens, diagnostics);
                    return new NamedParameterSyntax(paramsKeyword, varKeyword, name,
                        colon, typeExpression, equals, defaultValue);
            }
        }
    }
}
