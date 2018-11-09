using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using VarKeywordToken = Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.VarKeywordToken;

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
                case MutableKeywordToken mutableKeyword:
                    {
                        tokens.MoveNext();
                        var selfKeyword = tokens.Expect<SelfKeywordToken>();
                        return new SelfParameterSyntax(mutableKeyword, selfKeyword);
                    }
                case SelfKeywordToken selfKeyword:
                    tokens.MoveNext();
                    return new SelfParameterSyntax(null, selfKeyword);
                default:
                    var paramsKeyword = tokens.Accept<ParamsKeywordToken>();
                    var varKeyword = tokens.Accept<VarKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var colon = tokens.Expect<IColonToken>();
                    // Need to not consume the assignment that separates the type from the default value,
                    // hence the min operator precedence.
                    var typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.AboveAssignment);
                    var equals = tokens.Accept<EqualsToken>();
                    ExpressionSyntax defaultValue = null;
                    if (equals != null)
                        defaultValue = expressionParser.ParseExpression(tokens, diagnostics);
                    return new NamedParameterSyntax(paramsKeyword, varKeyword, name,
                        colon, typeExpression, equals, defaultValue);
            }
        }
    }
}
