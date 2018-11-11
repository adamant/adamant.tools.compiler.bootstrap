using System.Collections.Generic;
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
            var openBracket = tokens.Accept<IOpenBracketToken>();
            if (openBracket == null) return null;
            var parameters = listParser.ParseSeparatedList(tokens, ParseGenericParameter,
                TypeOf<ICommaToken>(), TypeOf<ICloseBracketToken>(), diagnostics);
            var closeBracket = tokens.Consume<ICloseBracketTokenPlace>();
            return parameters.OfType<GenericParameterSyntax>().ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public GenericParameterSyntax ParseGenericParameter(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var paramsKeyword = tokens.Accept<IParamsKeywordToken>();
            var name = tokens.ExpectIdentifier();
            var colon = tokens.Accept<IColonToken>();
            var typeExpression = colon != null ? expressionParser.ParseExpression(tokens, diagnostics) : null;
            return new GenericParameterSyntax(paramsKeyword, name, colon, typeExpression);
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<GenericConstraintSyntax> ParseGenericConstraints(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var constraints = new List<GenericConstraintSyntax>();
            while (tokens.Current is IWhereKeywordToken)
                constraints.Add(ParseGenericConstraint(tokens, diagnostics));

            return constraints.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public GenericConstraintSyntax ParseGenericConstraint(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var whereKeyword = tokens.Take<IWhereKeywordToken>();
            var expression = expressionParser.ParseExpression(tokens, diagnostics);
            return new GenericConstraintSyntax(whereKeyword, expression);
        }
    }
}
