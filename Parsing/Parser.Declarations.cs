using System;
using System.Collections.Generic;
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
                    return ParseMethod(classDeclaration, modifiers);
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
            var parameters = bodyParser.ParseParameters();
            var lifetimeBounds = bodyParser.ParseLifetimeBounds();
            ITypeSyntax? returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseType();

            var body = bodyParser.ParseFunctionBody();
            var span = TextSpan.Covering(fn, body.Span);

            return new FunctionDeclarationSyntax(span, File, modifiers, name, identifier.Span,
                parameters, lifetimeBounds, returnType, body);
        }

        private FixedList<IParameterSyntax> ParseParameters()
        {
            Tokens.Expect<IOpenParenToken>();
            return ParseRestOfParameters();
        }

        /// <summary>
        /// Requires that the open paren has already been consumed
        /// </summary>
        /// <returns></returns>
        private FixedList<IParameterSyntax> ParseRestOfParameters()
        {
            var parameters = new List<IParameterSyntax>();
            while (!Tokens.AtEnd<ICloseParenToken>())
            {
                parameters.Add(ParseParameter());
                if (Tokens.Current is ICommaToken)
                    Tokens.Expect<ICommaToken>();
            }

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
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
            return new BodySyntax(span, statements);
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
            var binding = Tokens.Expect<IBindingToken>();
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

        internal MethodDeclarationSyntax ParseMethod(
            IClassDeclarationSyntax declaringType,
            FixedList<IModiferToken> modifiers)
        {
            var fn = Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            var lifetimeBounds = bodyParser.ParseLifetimeBounds();
            ITypeSyntax? returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseType();

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
            var parameters = bodyParser.ParseParameters();
            var body = bodyParser.ParseFunctionBody();
            var span = TextSpan.Covering(newKeywordSpan, body.Span);
            return new ConstructorDeclarationSyntax(declaringType, span, File, modifiers, name,
                TextSpan.Covering(newKeywordSpan, identifier?.Span), parameters, body);
        }
        #endregion
    }
}
