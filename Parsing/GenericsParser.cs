using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class GenericsParser : Parser, IGenericsParser
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IExpressionParser expressionParser;

        public GenericsParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser)
            : base(tokens)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public FixedList<GenericParameterSyntax> AcceptGenericParameters()
        {
            if (!Tokens.Accept<IOpenBracketToken>()) return null;
            var parameters = listParser.ParseSeparatedList(Tokens, (tokens1, diagnostics1) => ParseGenericParameter(),
                TypeOf<ICommaToken>(), TypeOf<ICloseBracketToken>(), Tokens.Context.Diagnostics);
            Tokens.Expect<ICloseBracketToken>();
            return parameters.OfType<GenericParameterSyntax>().ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public GenericParameterSyntax ParseGenericParameter()
        {
            var isParams = Tokens.Accept<IParamsKeywordToken>();
            var name = Tokens.RequiredIdentifier();
            ExpressionSyntax typeExpression = null;
            if (Tokens.Accept<IColonToken>())
                typeExpression = expressionParser.ParseExpression();
            return new GenericParameterSyntax(isParams, name, typeExpression);
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
            var expression = expressionParser.ParseExpression();
            return new GenericConstraintSyntax(expression);
        }
    }
}
