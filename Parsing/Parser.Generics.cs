namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    //public partial class Parser
    //{
    //    public FixedList<GenericParameterSyntax> AcceptGenericParameters()
    //    {
    //        if (!Tokens.Accept<IOpenBracketToken>())
    //            return null;
    //        var parameters = ParseMany<GenericParameterSyntax, ICommaToken, ICloseBracketToken>(ParseGenericParameter);
    //        Tokens.Expect<ICloseBracketToken>();
    //        return parameters;
    //    }

    //    public GenericParameterSyntax ParseGenericParameter()
    //    {
    //        Name name;
    //        var dollar = Tokens.AcceptToken<IDollarToken>();
    //        if (dollar != null)
    //        {
    //            var lifetime = Tokens.RequiredToken<ILifetimeNameToken>();
    //            var span = TextSpan.Covering(dollar.Span, lifetime.Span);

    //            switch (lifetime)
    //            {
    //                case IIdentifierToken lifetimeIdentifier:
    //                    name = nameContext.Qualify(lifetimeIdentifier.Value);
    //                    break;
    //                case IRefKeywordToken _:
    //                    name = nameContext.Qualify(SpecialName.Ref);
    //                    break;
    //                case IOwnedKeywordToken _:
    //                    Add(ParseError.OwnedNotValidAsGenericLifetimeParameter(File, span));
    //                    // We just treat it as if they had written `$\owned`
    //                    name = nameContext.Qualify("owned");
    //                    break;
    //                default:
    //                    throw NonExhaustiveMatchException.For(lifetime);
    //            }

    //            return new GenericParameterSyntax(true, false, name, null);
    //        }

    //        var isParams = Tokens.Accept<IParamsKeywordToken>();
    //        var identifier = Tokens.RequiredToken<IIdentifierToken>();
    //        name = nameContext.Qualify(identifier.Value);
    //        ExpressionSyntax typeExpression = null;
    //        if (Tokens.Accept<IColonToken>())
    //            typeExpression = ParseExpression();
    //        return new GenericParameterSyntax(false, isParams, name, typeExpression);
    //    }

    //    public FixedList<GenericConstraintSyntax> ParseGenericConstraints()
    //    {
    //        return AcceptMany(AcceptGenericConstraint);
    //    }

    //    public GenericConstraintSyntax AcceptGenericConstraint()
    //    {
    //        if (!Tokens.Accept<IWhereKeywordToken>())
    //            return null;
    //        var expression = ParseExpression();
    //        return new GenericConstraintSyntax(expression);
    //    }
    //}
}
