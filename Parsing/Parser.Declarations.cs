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
        public FixedList<DeclarationSyntax> ParseTopLevelDeclarations()
        {
            var declarations = new List<DeclarationSyntax>();
            // Keep going to the end of the file
            while (!Tokens.AtEnd<IEndOfFileToken>())
                try
                {
                    declarations.Add(ParseDeclaration());
                }
                catch (ParseFailedException)
                {
                    // Ignore: we would have consumed something before failing, try to get the next declaration
                }

            return declarations.ToFixedList();
        }

        public FixedList<DeclarationSyntax> ParseDeclarations()
        {
            var declarations = new List<DeclarationSyntax>();
            // Stop at end of file or close brace that contains these declarations
            while (!Tokens.AtEnd<ICloseBraceToken>())
                try
                {
                    declarations.Add(ParseDeclaration());
                }
                catch (ParseFailedException)
                {
                    // Ignore: we would have consumed something before failing, try to get the next declaration
                }

            return declarations.ToFixedList();
        }

        public DeclarationSyntax ParseDeclaration()
        {
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case INamespaceKeywordToken _:
                    return ParseNamespaceDeclaration(modifiers);
                case IClassKeywordToken _:
                    return ParseClass(modifiers);
                case IFunctionKeywordToken _:
                    return (DeclarationSyntax)ParseNamedFunction(modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        public MemberDeclarationSyntax ParseMemberDeclaration()
        {
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case IFunctionKeywordToken _:
                    return (MemberDeclarationSyntax)ParseNamedFunction(modifiers);
                case INewKeywordToken _:
                    return (MemberDeclarationSyntax)ParseConstructor(modifiers);
                case ILetKeywordToken _:
                    return ParseField(false, modifiers);
                case IVarKeywordToken _:
                    return ParseField(true, modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        #region Parse Namespaces
        public NamespaceDeclarationSyntax ParseNamespaceDeclaration(
             FixedList<IModiferToken> modifiers)
        {
            // TODO generate errors for attributes or modifiers
            var ns = Tokens.Expect<INamespaceKeywordToken>();
            var globalQualifier = Tokens.AcceptToken<IColonColonToken>();
            var (name, nameSpan) = ParseNamespaceName();
            nameSpan = TextSpan.Covering(nameSpan, globalQualifier?.Span);
            Tokens.Expect<IOpenBraceToken>();
            var bodyParser = NestedParser(nameContext.Qualify(name));
            var usingDirectives = bodyParser.ParseUsingDirectives();
            var declarations = bodyParser.ParseDeclarations();
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

        #region Parse Type Parts
        private FixedList<MemberDeclarationSyntax> ParseMemberDeclarations()
        {
            return ParseMany<MemberDeclarationSyntax, ICloseBraceToken>(ParseMemberDeclaration);
        }

        private (FixedList<MemberDeclarationSyntax> members, TextSpan span) ParseTypeBody()
        {
            var openBrace = Tokens.Expect<IOpenBraceToken>();
            var members = ParseMemberDeclarations();
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
            return (members, span);
        }
        #endregion

        #region Parse Type Declarations
        private ClassDeclarationSyntax ParseClass(
             FixedList<IModiferToken> modifiers)
        {
            var @class = Tokens.Expect<IClassKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var (members, bodySpan) = bodyParser.ParseTypeBody();
            var span = TextSpan.Covering(@class, bodySpan);
            return new ClassDeclarationSyntax(span, File, modifiers, name, identifier.Span, members);
        }
        #endregion

        #region Parse Type Member Declarations
        private FieldDeclarationSyntax ParseField(
            bool mutableBinding,
            FixedList<IModiferToken> modifiers)
        {
            // We should only be called when there is a binding keyword
            var binding = Tokens.Expect<IBindingToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            TypeSyntax? type = null;
            if (Tokens.Accept<IColonToken>())
                type = ParseType();

            ExpressionSyntax? initializer = null;
            if (Tokens.Accept<IEqualsToken>())
                initializer = ParseExpression();

            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(binding, semicolon);
            return new FieldDeclarationSyntax(span, File, modifiers, mutableBinding, name,
                identifier.Span, type, initializer);
        }
        #endregion

        #region Parse Functions
        public INamedFunctionDeclarationSyntax ParseNamedFunction(
            FixedList<IModiferToken> modifiers)
        {
            var fn = Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            var lifetimeBounds = bodyParser.ParseLifetimeBounds();
            TypeSyntax? returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseType();

            FixedList<StatementSyntax>? body = null;
            TextSpan span;
            if (Tokens.Current is IOpenBraceToken)
            {
                (body, span) = bodyParser.ParseFunctionBody();
                span = TextSpan.Covering(fn, span);
            }
            else
            {
                var semicolon = bodyParser.Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(fn, semicolon);
            }

            return new NamedFunctionDeclarationSyntax(span, File, modifiers,
                name, identifier.Span,
                parameters, lifetimeBounds, returnType, body);
        }

        public IConstructorDeclarationSyntax ParseConstructor(
            FixedList<IModiferToken> modifiers)
        {
            var newKeywordSpan = Tokens.Expect<INewKeywordToken>();
            var identifier = Tokens.AcceptToken<IIdentifierToken>();
            var name = nameContext.Qualify(SpecialName.Constructor(identifier?.Value));
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            var (body, bodySpan) = bodyParser.ParseFunctionBody();
            var span = TextSpan.Covering(newKeywordSpan, bodySpan);
            return new ConstructorDeclarationSyntax(span, File, modifiers, name,
                TextSpan.Covering(newKeywordSpan, identifier?.Span), parameters, body);
        }

        private ExpressionSyntax? ParseLifetimeBounds()
        {
            switch (Tokens.Current)
            {
                case IRightArrowToken _: // No Lifetime Bounds
                case IOpenBraceToken _: // No return, starting body
                    return null;
                default:
                    // TODO maybe lifetime bounds should be treated as distinct from expressions
                    return AcceptExpression();
            }
        }

        private FixedList<ParameterSyntax> ParseParameters()
        {
            Tokens.Expect<IOpenParenToken>();
            return ParseRestOfParameters();
        }

        /// <summary>
        /// Requires that the open paren has already been consumed
        /// </summary>
        /// <returns></returns>
        private FixedList<ParameterSyntax> ParseRestOfParameters()
        {
            var parameters = new List<ParameterSyntax>();
            while (!Tokens.AtEnd<ICloseParenToken>())
            {
                parameters.Add(ParseParameter());
                if (Tokens.Current is ICommaToken)
                    Tokens.Expect<ICommaToken>();
            }

            Tokens.Expect<ICloseParenToken>();
            return parameters.ToFixedList();
        }

        private (FixedList<StatementSyntax>, TextSpan) ParseFunctionBody()
        {
            var openBrace = Tokens.Expect<IOpenBraceToken>();
            var statements = ParseMany<StatementSyntax, ICloseBraceToken>(ParseStatement);
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
            return (statements, span);
        }
        #endregion
    }
}
