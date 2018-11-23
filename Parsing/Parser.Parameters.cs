using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [NotNull]
        public ParameterSyntax ParseParameter()
        {
            switch (Tokens.Current)
            {
                case IRefKeywordToken _:
                case IMutableKeywordToken _:
                case ISelfKeywordToken _:
                {
                    var span = Tokens.Current.Span;
                    var refSelf = Tokens.Accept<IRefKeywordToken>();
                    var mutableSelf = Tokens.Accept<IMutableKeywordToken>();
                    var selfSpan = Tokens.Expect<ISelfKeywordToken>();
                    span = TextSpan.Covering(span, selfSpan);
                    return new SelfParameterSyntax(span, refSelf, mutableSelf);
                }
                case IDotToken _:
                {
                    var dot = Tokens.Expect<IDotToken>();
                    var name = Tokens.RequiredToken<IIdentifierToken>();
                    var equals = Tokens.AcceptToken<IEqualsToken>();
                    ExpressionSyntax defaultValue = null;
                    if (equals != null)
                        defaultValue = ParseExpression();
                    var span = TextSpan.Covering(dot, name.Span, defaultValue?.Span);
                    return new FieldParameterSyntax(span, name.Value, defaultValue);
                }
                default:
                {
                    var span = Tokens.Current.Span;
                    var isParams = Tokens.Accept<IParamsKeywordToken>();
                    var mutableBinding = Tokens.Accept<IVarKeywordToken>();
                    var name = Tokens.RequiredToken<IIdentifierToken>();
                    Tokens.Expect<IColonToken>();
                    // Need to not consume the assignment that separates the type from the default value,
                    // hence the min operator precedence.
                    var type = ParseExpression(OperatorPrecedence.AboveAssignment);
                    ExpressionSyntax defaultValue = null;
                    if (Tokens.Accept<IEqualsToken>())
                        defaultValue = ParseExpression();
                    span = TextSpan.Covering(span, type.Span, defaultValue?.Span);
                    return new NamedParameterSyntax(span, isParams, mutableBinding, name.Value, type, defaultValue);
                }
            }
        }
    }
}
