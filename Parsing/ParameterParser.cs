using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class ParameterParser : Parser, IParameterParser
    {
        [NotNull] private readonly IExpressionParser expressionParser;

        public ParameterParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] IExpressionParser expressionParser)
            : base(tokens)
        {
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public ParameterSyntax ParseParameter(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case IMutableKeywordToken mutableKeyword:
                {
                    tokens.Next();
                    var selfKeyword = tokens.Consume<ISelfKeywordTokenPlace>();
                    return new SelfParameterSyntax(mutableKeyword, selfKeyword);
                }
                case ISelfKeywordToken selfKeyword:
                    tokens.Next();
                    return new SelfParameterSyntax(null, selfKeyword);
                case IDotToken _:
                {
                    Tokens.Expect<IDotToken>();
                    var name = tokens.ExpectIdentifier();
                    var equals = tokens.Accept<IEqualsToken>();
                    ExpressionSyntax defaultValue = null;
                    if (equals != null)
                        defaultValue = expressionParser.ParseExpression(tokens, diagnostics);
                    // TODO capture correct values
                    return new FieldParameterSyntax(name.Span);
                }
                default:
                {
                    var paramsKeyword = tokens.Accept<IParamsKeywordToken>();
                    var varKeyword = tokens.Accept<IVarKeywordToken>();
                    var name = tokens.ExpectIdentifier();
                    var colon = tokens.Consume<IColonTokenPlace>();
                    // Need to not consume the assignment that separates the type from the default value,
                    // hence the min operator precedence.
                    var typeExpression = expressionParser.ParseExpression(tokens, diagnostics,
                        OperatorPrecedence.AboveAssignment);
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
}
