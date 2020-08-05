using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
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
            var modifiers = ParseModifiers();

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
            var modifiers = ParseModifiers();

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

        [SuppressMessage("Performance", "CA1826:Do not use Enumerable methods on indexable collections. Instead use the collection directly",
            Justification = "LastOrDefault() doesn't have a simple equivalent")]
        private ModifierParser ParseModifiers()
        {
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);
            var endOfModifiers = TokenFactory.EndOfFile(modifiers.LastOrDefault()?.Span.AtEnd() ?? Tokens.Current.Span.AtStart());
            var modifierTokens = new TokenIterator<IEssentialToken>(Tokens.Context, modifiers.Append<IEssentialToken>(endOfModifiers));
            return new ModifierParser(modifierTokens);
        }

        #region Parse Namespaces
        internal NamespaceDeclarationSyntax ParseNamespaceDeclaration(
             ModifierParser modifiers)
        {
            modifiers.ParseEndOfModifiers();
            var ns = Tokens.Expect<INamespaceKeywordToken>();
            var globalQualifier = Tokens.AcceptToken<IColonColonDotToken>();
            var (name, nameSpan) = ParseNamespaceName();
            nameSpan = TextSpan.Covering(nameSpan, globalQualifier?.Span);
            Tokens.Expect<IOpenBraceToken>();
            _ = containingNamespace
                ?? throw new InvalidOperationException("Namespace not nested inside a containing namespace");
            var bodyParser = NestedParser(nameContext.Qualify((MaybeQualifiedName)name.ToRootName()), containingNamespace.Qualify(name));
            var usingDirectives = bodyParser.ParseUsingDirectives();
            var declarations = bodyParser.ParseNonMemberDeclarations<ICloseBraceToken>();
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(ns, closeBrace);
            return new NamespaceDeclarationSyntax(span, File,
                globalQualifier != null, name, nameSpan, containingNamespace, usingDirectives, declarations);
        }

        private (NamespaceName, TextSpan) ParseNamespaceName()
        {
            var firstSegment = Tokens.RequiredToken<IIdentifierToken>();
            var span = firstSegment.Span;
            NamespaceName name = firstSegment.Value;

            while (Tokens.Accept<IDotToken>())
            {
                var (nameSegment, segmentSpan) = Tokens.ExpectToken<IIdentifierToken>();
                // We need the span to cover a trailing dot
                span = TextSpan.Covering(span, segmentSpan);
                if (nameSegment is null)
                    break;
                name = name.Qualify(nameSegment.Value);
            }

            return (name, span);
        }
        #endregion

        #region Parse Functions
        internal IFunctionDeclarationSyntax ParseFunction(ModifierParser modifiers)
        {
            var accessModifer = modifiers.ParseAccessModifier();
            modifiers.ParseEndOfModifiers();
            var fn = Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name, null);
            var parameters = bodyParser.ParseParameters(bodyParser.ParseFunctionParameter);
            var (returnType, reachabilityAnnotations) = ParseReturn();
            var body = bodyParser.ParseFunctionBody();
            var span = TextSpan.Covering(fn, body.Span);

            return new FunctionDeclarationSyntax(span, File, accessModifer, name, identifier.Span,
                parameters, returnType, reachabilityAnnotations, body);
        }

        private FixedList<TParameter> ParseParameters<TParameter>(Func<TParameter> parseParameter)
            where TParameter : class, IParameterSyntax
        {
            Tokens.Expect<IOpenParenToken>();
            var parameters = ParseManySeparated<TParameter, ICommaToken, ICloseParenToken>(parseParameter);
            Tokens.Expect<ICloseParenToken>();
            return parameters.ToFixedList();
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

        private (ITypeSyntax?, FixedList<IReachabilityAnnotationSyntax>) ParseReturn()
        {
            if (Tokens.Accept<IRightArrowToken>())
                return (ParseType(), ParseReachabilityAnnotations());

            return (null, FixedList<IReachabilityAnnotationSyntax>.Empty);
        }
        #endregion

        #region Parse Class Declarations
        private IClassDeclarationSyntax ParseClass(
             ModifierParser modifiers)
        {
            var accessModifier = modifiers.ParseAccessModifier();
            var mutableModifier = modifiers.ParseMutableModifier();
            modifiers.ParseEndOfModifiers();
            var @class = Tokens.Expect<IClassKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var headerSpan = TextSpan.Covering(@class, identifier.Span);
            var bodyParser = NestedParser(name, null);
            return new ClassDeclarationSyntax(headerSpan, File, accessModifier, mutableModifier, name, identifier.Span, bodyParser.ParseClassBody);
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
            ModifierParser modifiers)
        {
            var accessModifer = modifiers.ParseAccessModifier();
            modifiers.ParseEndOfModifiers();
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
            return new FieldDeclarationSyntax(declaringType, span, File, accessModifer, mutableBinding, name,
                identifier.Span, type, initializer);
        }

        internal IMemberDeclarationSyntax ParseMemberFunction(
            IClassDeclarationSyntax declaringType,
            ModifierParser modifiers)
        {
            var accessModifer = modifiers.ParseAccessModifier();
            modifiers.ParseEndOfModifiers();
            var fn = Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name, null);
            var parameters = bodyParser.ParseParameters(bodyParser.ParseMethodParameter);
            var (returnType, reachabilityAnnotations) = ParseReturn();

            var selfParameter = parameters.OfType<ISelfParameterSyntax>().FirstOrDefault();
            var namedParameters = parameters.Except(parameters.OfType<ISelfParameterSyntax>())
                                            .Cast<INamedParameterSyntax>().ToFixedList();

            // if no self parameter, it is an associated function
            if (selfParameter is null)
            {
                var body = bodyParser.ParseFunctionBody();
                var span = TextSpan.Covering(fn, body.Span);
                return new AssociatedFunctionDeclarationSyntax(declaringType, span, File, accessModifer,
                    name, identifier.Span, namedParameters, returnType, reachabilityAnnotations, body);
            }

            if (!(parameters[0] is ISelfParameterSyntax))
                Add(ParseError.SelfParameterMustBeFirst(File, selfParameter.Span));

            foreach (var extraSelfParameter in parameters.OfType<ISelfParameterSyntax>().Skip(1))
                Add(ParseError.ExtraSelfParameter(File, extraSelfParameter.Span));

            // It is a method that may or may not have a body
            if (Tokens.Current is IOpenBraceToken)
            {
                var body = bodyParser.ParseFunctionBody();
                var span = TextSpan.Covering(fn, body.Span);
                return new ConcreteMethodDeclarationSyntax(declaringType, span, File, accessModifer, name,
                    identifier.Span, selfParameter, namedParameters, returnType, reachabilityAnnotations, body);
            }
            else
            {
                var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
                var span = TextSpan.Covering(fn, semicolon);
                return new AbstractMethodDeclarationSyntax(declaringType, span, File, accessModifer, name,
                    identifier.Span, selfParameter, namedParameters, returnType, reachabilityAnnotations);
            }
        }

        internal ConstructorDeclarationSyntax ParseConstructor(
            IClassDeclarationSyntax declaringType,
            ModifierParser modifiers)
        {
            var accessModifer = modifiers.ParseAccessModifier();
            modifiers.ParseEndOfModifiers();
            var newKeywordSpan = Tokens.Expect<INewKeywordToken>();
            var identifier = Tokens.AcceptToken<IIdentifierToken>();
            var name = nameContext.Qualify(SpecialNames.Constructor(identifier?.Value));
            var bodyParser = NestedParser(name, null);
            // Implicit self parameter is taken to be after the current token which is expected to be `(`
            var selfParameter = new SelfParameterSyntax(Tokens.Current.Span.AtEnd(), name.Qualify(SpecialNames.Self), true);
            var parameters = bodyParser.ParseParameters(bodyParser.ParseConstructorParameter);
            var body = bodyParser.ParseFunctionBody();
            // For now, just say constructors have no annotations
            var reachabilityAnnotations = FixedList<IReachabilityAnnotationSyntax>.Empty;
            var span = TextSpan.Covering(newKeywordSpan, body.Span);
            return new ConstructorDeclarationSyntax(declaringType, span, File, accessModifer, name,
                TextSpan.Covering(newKeywordSpan, identifier?.Span), selfParameter, parameters, reachabilityAnnotations, body);
        }
        #endregion
    }
}
