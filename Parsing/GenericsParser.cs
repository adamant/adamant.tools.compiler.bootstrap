using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class GenericsParser : IGenericsParser
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IExpressionParser expressionParser;

        public GenericsParser(
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public FixedList<GenericParameterSyntax> AcceptGenericParameters(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            if (!tokens.Accept<IOpenBracketToken>()) return null;
            var parameters = listParser.ParseSeparatedList(tokens, ParseGenericParameter,
                TypeOf<ICommaToken>(), TypeOf<ICloseBracketToken>(), diagnostics);
            tokens.Expect<ICloseBracketToken>();
            return parameters.OfType<GenericParameterSyntax>().ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public GenericParameterSyntax ParseGenericParameter(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var isParams = tokens.Accept<IParamsKeywordToken>();
            var name = tokens.RequiredIdentifier();
            ExpressionSyntax typeExpression = null;
            if (tokens.Accept<IColonToken>())
                typeExpression = expressionParser.ParseExpression(tokens, diagnostics);
            return new GenericParameterSyntax(isParams, name, typeExpression);
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<GenericConstraintSyntax> ParseGenericConstraints(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            return listParser.AcceptList(() => AcceptGenericConstraint(tokens, diagnostics));
        }

        [MustUseReturnValue]
        [CanBeNull]
        public GenericConstraintSyntax AcceptGenericConstraint(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            if (!tokens.Accept<IWhereKeywordToken>()) return null;
            var expression = expressionParser.ParseExpression(tokens, diagnostics);
            return new GenericConstraintSyntax(expression);
        }
    }
}
