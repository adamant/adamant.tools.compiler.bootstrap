using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [CanBeNull]
        public FixedList<GenericParameterSyntax> AcceptGenericParameters()
        {
            if (!Tokens.Accept<IOpenBracketToken>()) return null;
            var parameters = listParser.ParseSeparatedList<GenericParameterSyntax, ICommaToken, ICloseBracketToken>(ParseGenericParameter);
            Tokens.Expect<ICloseBracketToken>();
            return parameters;
        }

        [MustUseReturnValue]
        [NotNull]
        public GenericParameterSyntax ParseGenericParameter()
        {
            var isParams = Tokens.Accept<IParamsKeywordToken>();
            var name = Tokens.RequiredToken<IIdentifierToken>();
            ExpressionSyntax typeExpression = null;
            if (Tokens.Accept<IColonToken>())
                typeExpression = ParseExpression();
            return new GenericParameterSyntax(isParams, name.Value, typeExpression);
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<GenericConstraintSyntax> ParseGenericConstraints()
        {
            return listParser.AcceptList(AcceptGenericConstraint);
        }

        [MustUseReturnValue]
        [CanBeNull]
        public GenericConstraintSyntax AcceptGenericConstraint()
        {
            if (!Tokens.Accept<IWhereKeywordToken>()) return null;
            var expression = ParseExpression();
            return new GenericConstraintSyntax(expression);
        }
    }
}
