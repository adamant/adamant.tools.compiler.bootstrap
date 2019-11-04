using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public FixedList<INonMemberDeclarationSyntax> ParseNonMemberDeclarations<T>()
            where T : IToken
        {
            var declarations = new List<INonMemberDeclarationSyntax>();
            // Stop at end of file or some token that contains these declarations
            while (!Tokens.AtEnd<T>())
                try
                {
                    declarations.Add(ParseNonMemberDeclaration());
                }
                catch (ParseFailedException)
                {
                    // Ignore: we would have consumed something before failing, try to get the next declaration
                }

            return declarations.ToFixedList();
        }

        public INonMemberDeclarationSyntax ParseNonMemberDeclaration()
        {
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case INamespaceKeywordToken _:
                    return ParseNamespaceDeclaration(modifiers);
                case IClassKeywordToken _:
                    return ParseClass(modifiers);
                case IFunctionKeywordToken _:
                    return ParseFunction(modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        internal IMemberDeclarationSyntax ParseMemberDeclaration(
            IClassDeclarationSyntax classDeclaration)
        {
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case IFunctionKeywordToken _:
                    return ParseMemberFunction(classDeclaration, modifiers);
                case INewKeywordToken _:
                    return ParseConstructor(classDeclaration, modifiers);
                case ILetKeywordToken _:
                    return ParseField(classDeclaration, false, modifiers);
                case IVarKeywordToken _:
                    return ParseField(classDeclaration, true, modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        #region Parse Namespaces
        internal NamespaceDeclarationSyntax ParseNamespaceDeclaration(
             FixedList<IModiferToken> _)
        {
            // TODO generate errors for attributes or modifiers
            var ns = Tokens.Expect<INamespaceKeywordToken>();
            var globalQualifier = Tokens.AcceptToken<IColonColonDotToken>();
            var (name, nameSpan) = ParseNamespaceName();
            nameSpan = TextSpan.Covering(nameSpan, globalQualifier?.Span);
            Tokens.Expect<IOpenBraceToken>();
            var bodyParser = NestedParser(nameContext.Qualify(name));
            var usingDirectives = bodyParser.ParseUsingDirectives();
            var declarations = bodyParser.ParseNonMemberDeclarations<ICloseBraceToken>();
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(ns, closeBrace);
            return new NamespaceDeclarationSyntax(span, File,
                globalQualifier != null, name, nameSpan,
                nameContext, usingDirectives, declarations);
        }

        private (Name, TextSpan) ParseNamespaceName()
        {
            var firstSegment = Tokens.RequiredToken<IIdentifierToken>();
            var span = firstSegment.Span;
            Name name = new SimpleName(firstSegment.Value);

            while (Tokens.Accept<IDotToken>())
            {
                var (nameSegment, segmentSpan) = Tokens.ExpectIdentifier();
                // We need the span to cover a trailing dot
                span = TextSpan.Covering(span, segmentSpan);
                if (nameSegment == null)
                    break;
                name = name.Qualify(nameSegment.Value);
            }

            return (name, span);
        }
        #endregion

        #region Parse Functions
        public IFunctionDeclarationSyntax ParseFunction(FixedList<IModiferToken> modifiers)
        {
            var fn = Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters(ParseFunctionParameter);
            var lifetimeBounds = bodyParser.ParseLifetimeBounds();
            ITypeSyntax? returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseType();

            var body = bodyParser.ParseFunctionBody();
            var span = TextSpan.Covering(fn, body.Span);

            return new FunctionDeclarationSyntax(span, File, modifiers, name, identifier.Span,
                parameters, lifetimeBounds, returnType, body);
        }

        private FixedList<TParameter> ParseParameters<TParameter>(Func<TParameter> parseParameter)
            where TParameter : class, IParameterSyntax
        {
            Tokens.Expect<IOpenParenToken>();
            var parameters = ParseMany<TParameter, ICommaToken, ICloseParenToken>(parseParameter);
            Tokens.Expect<ICloseParenToken>();
            return parameters.ToFixedList();
        }

        private ILifetimeBoundSyntax? ParseLifetimeBounds()
        {
            switch (Tokens.Current)
            {
                case IRightArrowToken _: // No Lifetime Bounds
                case IOpenBraceToken _: // No return, starting body
                    return null;
                case IDollarToken _:
                    return ParseParameterLifetimeBound();
                default:
                    throw new NotImplementedException();
                    // TODO maybe lifetime bounds should be treated as distinct from expressions
                    //return AcceptExpression();
            }
        }

        private IParameterLifetimeBoundSyntax ParseParameterLifetimeBound()
        {
            var dollar = Tokens.Expect<IDollarToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var span = TextSpan.Covering(dollar, identifier.Span);
            return new ParameterLifetimeBoundSyntax(span, new SimpleName(identifier.Value));
        }

        private IBodySyntax ParseFunctionBody()
        {
            var openBrace = Tokens.Expect<IOpenBraceToken>();
            var statements = ParseMany<IStatementSyntax, ICloseBraceToken>(ParseStatement);
            foreach (var resultStatement in statements.OfType<IResultStatementSyntax>())
                Add(ParseError.ResultStatementInBody(File, resultStatement.Span));
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
            return new BodySyntax(span, statements.OfType<IBodyStatementSyntax>().ToFixedList());
        }
        #endregion

        #region Parse Class Declarations
        private IClassDeclarationSyntax ParseClass(
             FixedList<IModiferToken> modifiers)
        {
            var @class = Tokens.Expect<IClassKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var headerSpan = TextSpan.Covering(@class, identifier.Span);
            var bodyParser = NestedParser(name);
            return new ClassDeclarationSyntax(headerSpan, File, modifiers, name, identifier.Span, bodyParser.ParseClassBody);
        }

        private (FixedList<IMemberDeclarationSyntax> members, TextSpan span) ParseClassBody(IClassDeclarationSyntax declaringType)
        {
            var openBrace = Tokens.Expect<IOpenBraceToken>();
            var members = ParseMemberDeclarations(declaringType);
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
            return (members, span);
        }
        #endregion

        #region Parse Member Declarations
        private FixedList<IMemberDeclarationSyntax> ParseMemberDeclarations(
            IClassDeclarationSyntax declaringType)
        {
            return ParseMany<IMemberDeclarationSyntax, ICloseBraceToken>(() => ParseMemberDeclaration(declaringType));
        }

        internal FieldDeclarationSyntax ParseField(
            IClassDeclarationSyntax declaringType,
            bool mutableBinding,
            FixedList<IModiferToken> modifiers)
        {
            // We should only be called when there is a binding keyword
            var binding = Tokens.Required<IBindingToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            Tokens.Expect<IColonToken>();
            var type = ParseType();
            IExpressionSyntax? initializer = null;
            if (Tokens.Accept<IEqualsToken>())
                initializer = ParseExpression();

            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(binding, semicolon);
            return new FieldDeclarationSyntax(declaringType, span, File, modifiers, mutableBinding, name,
                identifier.Span, type, initializer);
        }

        internal IMemberDeclarationSyntax ParseMemberFunction(
            IClassDeclarationSyntax declaringType,
            FixedList<IModiferToken> modifiers)
        {
            var fn = Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters(ParseMethodParameter);
            var lifetimeBounds = bodyParser.ParseLifetimeBounds();
            ITypeSyntax? returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseType();

            // if no self parameter, it is an associated function
            if (!parameters.OfType<ISelfParameterSyntax>().Any())
            {
                var namedParameters = parameters.OfType<INamedParameterSyntax>().ToFixedList();
                var body = bodyParser.ParseFunctionBody();
                var span = TextSpan.Covering(fn, body.Span);
                return new AssociatedFunctionDeclarationSyntax(declaringType, span, File, modifiers,
                    name, identifier.Span, namedParameters, lifetimeBounds, returnType, body);
            }

            if (!(parameters[0] is ISelfParameterSyntax))
                Add(ParseError.FirstParameterMustBeSelf(File, parameters[0].Span));

            foreach (var selfParameter in parameters.Skip(1).OfType<ISelfParameterSyntax>())
                Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

            // It is a method that may or may not have a body
            if (Tokens.Current is IOpenBraceToken)
            {
                var body = bodyParser.ParseFunctionBody();
                var span = TextSpan.Covering(fn, body.Span);
                return new ConcreteMethodDeclarationSyntax(declaringType, span, File, modifiers, name,
                    identifier.Span, parameters, lifetimeBounds, returnType, body);
            }
            else
            {
                var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
                var span = TextSpan.Covering(fn, semicolon);
                return new AbstractMethodDeclarationSyntax(declaringType, span, File, modifiers, name,
                    identifier.Span, parameters, lifetimeBounds, returnType);
            }
        }

        internal ConstructorDeclarationSyntax ParseConstructor(
            IClassDeclarationSyntax declaringType,
            FixedList<IModiferToken> modifiers)
        {
            var newKeywordSpan = Tokens.Expect<INewKeywordToken>();
            var identifier = Tokens.AcceptToken<IIdentifierToken>();
            var name = nameContext.Qualify(SpecialName.Constructor(identifier?.Value));
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters(ParseConstructorParameter);
            var body = bodyParser.ParseFunctionBody();
            var span = TextSpan.Covering(newKeywordSpan, body.Span);
            return new ConstructorDeclarationSyntax(declaringType, span, File, modifiers, name,
                TextSpan.Covering(newKeywordSpan, identifier?.Span), parameters, body);
        }
        #endregion
    }
}
