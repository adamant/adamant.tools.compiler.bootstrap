using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
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
                    declarations.AddRange(ParseDeclaration());
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
                    declarations.AddRange(ParseDeclaration());
                }
                catch (ParseFailedException)
                {
                    // Ignore: we would have consumed something before failing, try to get the next declaration
                }

            return declarations.ToFixedList();
        }

        public IEnumerable<DeclarationSyntax> ParseDeclaration()
        {
            //var attributes = ParseAttributes();
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case INamespaceKeywordToken _:
                    return ParseNamespaceDeclaration(/*attributes,*/ modifiers).Yield();
                case IClassKeywordToken _:
                    return ParseClass(/*attributes,*/ modifiers).Yield();
                //case ITraitKeywordToken _:
                //    return ParseTrait(attributes, modifiers).Yield();
                //case IStructKeywordToken _:
                //    return ParseStruct(attributes, modifiers).Yield();
                //case IEnumKeywordToken _:
                //    return ParseEnum(attributes, modifiers).Yield();
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(/*attributes,*/ modifiers).Yield();
                //case IConstKeywordToken _:
                //    return ParseConst(attributes, modifiers).Yield();
                //case IExternalKeywordToken _:
                //    return ParseExternalBlock(attributes, modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        /// <summary>
        /// Skip tokens until we reach what we assume to be the end of a statement
        /// </summary>
        // TODO does this belong on a statement parser or something
        private void SkipToEndOfStatement()
        {
            while (!Tokens.AtEnd<ISemicolonToken>())
                Tokens.Next();

            // Consume the semicolon is we aren't at the end of the file.
            var _ = Tokens.Accept<ISemicolonToken>();
        }

        public MemberDeclarationSyntax ParseMemberDeclaration()
        {
            //var attributes = ParseAttributes();
            var modifiers = AcceptMany(Tokens.AcceptToken<IModiferToken>);

            switch (Tokens.Current)
            {
                case IClassKeywordToken _:
                    return ParseClass(/*attributes,*/ modifiers);
                //case ITraitKeywordToken _:
                //    return ParseTrait(attributes, modifiers);
                //case IStructKeywordToken _:
                //    return ParseStruct(attributes, modifiers);
                //case IEnumKeywordToken _:
                //    return ParseEnum(attributes, modifiers);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(/*attributes,*/ modifiers);
                //case IOperatorKeywordToken _:
                //    return ParseOperator(attributes, modifiers);
                case INewKeywordToken _:
                    return ParseConstructor(/*attributes,*/ modifiers);
                //case IInitKeywordToken _:
                //    return ParseInitializer(attributes, modifiers);
                //case IDeleteKeywordToken _:
                //    return ParseDestructor(attributes, modifiers);
                //case IGetKeywordToken _:
                //    return ParseGetter(attributes, modifiers);
                //case ISetKeywordToken _:
                //    return ParseSetter(attributes, modifiers);
                case ILetKeywordToken _:
                    return ParseField(false,/* attributes,*/ modifiers);
                case IVarKeywordToken _:
                    return ParseField(true, /*attributes,*/ modifiers);
                //case IConstKeywordToken _:
                //    return ParseConst(attributes, modifiers);
                default:
                    Tokens.UnexpectedToken();
                    throw new ParseFailedException();
            }
        }

        #region Parse Namespaces
        public NamespaceDeclarationSyntax ParseNamespaceDeclaration(
             //FixedList<AttributeSyntax> attributes,
             FixedList<IModiferToken> modifiers)
        {
            // TODO generate errors for attributes or modifiers
            Tokens.Expect<INamespaceKeywordToken>();
            var globalQualifier = Tokens.AcceptToken<IColonColonToken>();
            var (name, span) = ParseNamespaceName();
            span = TextSpan.Covering(span, globalQualifier?.Span);
            Tokens.Expect<IOpenBraceToken>();
            var bodyParser = NestedParser(nameContext.Qualify(name));
            var usingDirectives = bodyParser.ParseUsingDirectives();
            var declarations = bodyParser.ParseDeclarations();
            Tokens.Expect<ICloseBraceToken>();
            return new NamespaceDeclarationSyntax(File, globalQualifier != null, name, span,
                nameContext, usingDirectives, declarations);
        }

        private (Name, TextSpan) ParseNamespaceName()
        {
            var nameSegment = Tokens.RequiredToken<IIdentifierToken>();
            var span = nameSegment.Span;
            Name name = new SimpleName(nameSegment.Value);

            while (Tokens.Accept<IDotToken>())
            {
                TextSpan segmentSpan;
                (segmentSpan, nameSegment) = Tokens.ExpectIdentifier();
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
        //private FixedList<AttributeSyntax> ParseAttributes()
        //{
        //    var attributes = new List<AttributeSyntax>();
        //    // Take modifiers until null
        //    while (AcceptAttribute() is AttributeSyntax attribute)
        //        attributes.Add(attribute);
        //    return attributes.ToFixedList();
        //}

        //private AttributeSyntax AcceptAttribute()
        //{
        //    if (!Tokens.Accept<IHashHashToken>())
        //        return null;
        //    var name = ParseName();
        //    FixedList<ArgumentSyntax> arguments = null;
        //    if (Tokens.Accept<IOpenParenToken>())
        //    {
        //        arguments = ParseArguments();
        //        Tokens.Expect<ICloseParenToken>();
        //    }
        //    return new AttributeSyntax(name, arguments);
        //}

        //private ExpressionSyntax AcceptBaseClass()
        //{
        //    if (!Tokens.Accept<IColonToken>())
        //        return null;
        //    return ParseExpression();
        //}

        //private FixedList<ExpressionSyntax> AcceptBaseTypes()
        //{
        //    if (!Tokens.Accept<ILessThanColonToken>())
        //        return null;
        //    return AcceptOneOrMore<ExpressionSyntax, ICommaToken>(ParseExpression);
        //}

        //private FixedList<ExpressionSyntax> ParseInvariants()
        //{
        //    return AcceptMany(AcceptInvariant);
        //}

        //private ExpressionSyntax AcceptInvariant()
        //{
        //    if (!Tokens.Accept<IInvariantKeywordToken>())
        //        return null;
        //    return ParseExpression();
        //}

        private FixedList<MemberDeclarationSyntax> ParseMemberDeclarations()
        {
            return ParseMany<MemberDeclarationSyntax, ICloseBraceToken>(ParseMemberDeclaration);
        }

        private FixedList<MemberDeclarationSyntax> ParseTypeBody()
        {
            Tokens.Expect<IOpenBraceToken>();
            var members = ParseMemberDeclarations();
            Tokens.Expect<ICloseBraceToken>();
            return members;
        }
        #endregion

        #region Parse Type Declarations
        private ClassDeclarationSyntax ParseClass(
             FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<IClassKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var members = bodyParser.ParseTypeBody();
            return new ClassDeclarationSyntax(File, modifiers, name, identifier.Span, members);
        }
        #endregion

        #region Parse Type Member Declarations
        private FieldDeclarationSyntax ParseField(
            bool mutableBinding,
            //FixedList<AttributeSyntax> attributes,
            FixedList<IModiferToken> modifiers)
        {
            // We should only be called when there is a binding keyword
            Tokens.Expect<IBindingToken>();
            //var getterAccess = AcceptFieldGetter();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            ExpressionSyntax typeExpression = null;
            if (Tokens.Accept<IColonToken>())
            {
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = ParseExpression(OperatorPrecedence.AboveAssignment);
            }
            ExpressionSyntax initializer = null;
            if (Tokens.Accept<IEqualsToken>())
                initializer = ParseExpression();

            Tokens.Expect<ISemicolonToken>();
            return new FieldDeclarationSyntax(File, /*attributes,*/ modifiers, mutableBinding, /*getterAccess,*/ name,
                identifier.Span, typeExpression, initializer);
        }
        #endregion

        #region Parse Functions
        public NamedFunctionDeclarationSyntax ParseNamedFunction(
            FixedList<IModiferToken> modifiers)
        {
            Tokens.Expect<IFunctionKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(identifier.Value);
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            var lifetimeBounds = bodyParser.ParseLifetimeBounds();
            ExpressionSyntax returnType = null;
            if (Tokens.Accept<IRightArrowToken>())
                returnType = ParseExpression();
            var body = bodyParser.ParseFunctionBody();
            return new NamedFunctionDeclarationSyntax(File, modifiers, name, identifier.Span,
                parameters, lifetimeBounds, returnType, body);
        }

        public ConstructorDeclarationSyntax ParseConstructor(
            FixedList<IModiferToken> modifiers)
        {
            var newKeywordSpan = Tokens.Expect<INewKeywordToken>();
            var identifier = Tokens.AcceptToken<IIdentifierToken>();
            var name = nameContext.Qualify(SpecialName.Constructor(identifier?.Value));
            var bodyParser = NestedParser(name);
            var parameters = bodyParser.ParseParameters();
            var body = bodyParser.ParseBlock();
            return new ConstructorDeclarationSyntax(File, modifiers, name,
                TextSpan.Covering(newKeywordSpan, identifier?.Span), parameters, body);
        }

        private ExpressionSyntax ParseLifetimeBounds()
        {
            switch (Tokens.Current)
            {
                case IRightArrowToken _: // No Lifetime Bounds
                case IOpenBraceToken _: // No return, starting body
                    return null;
                default:
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

        private BlockSyntax ParseFunctionBody()
        {
            var body = AcceptBlock();
            if (body == null)
                Tokens.Expect<ISemicolonToken>();
            return body;
        }
        #endregion
    }
}
