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
                case IRefKeywordToken _:
                case IMutableKeywordToken _:
                case ISelfKeywordToken _:
                {
                    var span = tokens.Current.Span;
                    var isRef = tokens.Accept<IRefKeywordToken>();
                    var mutableBinding = tokens.Accept<IMutableKeywordToken>();
                    var selfSpan = tokens.Expect<ISelfKeywordToken>();
                    span = TextSpan.Covering(span, selfSpan);
                    return new SelfParameterSyntax(span, isRef, mutableBinding);
                }
                case IDotToken _:
                {
                    Tokens.Expect<IDotToken>();
                    var name = tokens.ExpectIdentifier();
                    var equals = tokens.AcceptToken<IEqualsToken>();
                    ExpressionSyntax defaultValue = null;
                    if (equals != null)
                        defaultValue = expressionParser.ParseExpression(tokens, diagnostics);
                    // TODO capture correct values
                    return new FieldParameterSyntax(name.Span);
                }
                default:
                {
                    var span = tokens.Current.Span;
                    var isParams = tokens.Accept<IParamsKeywordToken>();
                    var mutableBinding = tokens.Accept<IVarKeywordToken>();
                    var name = tokens.RequiredIdentifier();
                    tokens.Expect<IColonToken>();
                    // Need to not consume the assignment that separates the type from the default value,
                    // hence the min operator precedence.
                    var type = expressionParser.ParseExpression(tokens, diagnostics,
                        OperatorPrecedence.AboveAssignment);
                    ExpressionSyntax defaultValue = null;
                    if (tokens.Accept<IEqualsToken>())
                        defaultValue = expressionParser.ParseExpression(tokens, diagnostics);
                    span = TextSpan.Covering(span, type.Span, defaultValue?.Span);
                    return new NamedParameterSyntax(span, isParams, mutableBinding, name, type, defaultValue);
                }
            }
        }
    }
}
