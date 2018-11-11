using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class DeclarationParser : Parser, IDeclarationParser
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IExpressionParser expressionParser;
        [NotNull] private readonly IBlockParser blockParser;
        [NotNull] private readonly IParameterParser parameterParser;
        [NotNull] private readonly IModifierParser modifierParser;
        [NotNull] private readonly IGenericsParser genericsParser;
        [NotNull] private readonly INameParser nameParser;
        [NotNull] private readonly IUsingDirectiveParser usingDirectiveParser;

        public DeclarationParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] IListParser listParser,
            [NotNull] IExpressionParser expressionParser,
            [NotNull] IBlockParser blockParser,
            [NotNull] IParameterParser parameterParser,
            [NotNull] IModifierParser modifierParser,
            [NotNull] IGenericsParser genericsParser,
            [NotNull] INameParser nameParser,
            [NotNull] IUsingDirectiveParser usingDirectiveParser)
            : base(tokens)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
            this.blockParser = blockParser;
            this.parameterParser = parameterParser;
            this.modifierParser = modifierParser;
            this.genericsParser = genericsParser;
            this.nameParser = nameParser;
            this.usingDirectiveParser = usingDirectiveParser;
        }

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<DeclarationSyntax> ParseDeclarations()
        {
            var declarations = new List<DeclarationSyntax>();
            while (!Tokens.AtEndOfFile() && !(Tokens.Current is ICloseBraceToken))
                declarations.Add(ParseDeclaration());

            return declarations.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public DeclarationSyntax ParseDeclaration()
        {
            var attributes = ParseAttributes();
            var modifiers = listParser.ParseList(Tokens.Accept<IModiferToken>);

            switch (Tokens.Current)
            {
                case INamespaceKeywordToken _:
                    return ParseNamespaceDeclaration(attributes, modifiers);
                case IClassKeywordToken _:
                    return ParseClass(attributes, modifiers);
                case ITraitKeywordToken _:
                    return ParseTrait(attributes, modifiers);
                case IStructKeywordToken _:
                    return ParseStruct(attributes, modifiers);
                case IEnumKeywordToken _:
                    return ParseEnum(attributes, modifiers);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(attributes, modifiers);
                //case IConstKeywordToken _:
                //    return ParseConst(attributes, modifiers);
                //default:
                //    return ParseIncompleteDeclaration(attributes, modifiers);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        public MemberDeclarationSyntax ParseMemberDeclaration()
        {
            var attributes = ParseAttributes();
            var modifiers = listParser.ParseList(Tokens.Accept<IModiferToken>);

            switch (Tokens.Current)
            {
                //case IClassKeywordToken _:
                //    return ParseClass(attributes, modifiers);
                //case ITypeKeywordToken _:
                //    return ParseType(attributes, modifiers);
                case IStructKeywordToken _:
                    return ParseStruct(attributes, modifiers);
                //case IEnumKeywordToken _:
                //    return ParseEnum(attributes, modifiers);
                case IFunctionKeywordToken _:
                    return ParseNamedFunction(attributes, modifiers);
                case IOperatorKeywordToken _:
                    return ParseOperatorFunction(attributes, modifiers);
                case INewKeywordToken _:
                    return ParseConstructor(attributes, modifiers);
                case IInitKeywordToken _:
                    return ParseInitializer(attributes, modifiers);
                case IDeleteKeywordToken _:
                    return ParseDestructor(attributes, modifiers);
                //case IGetKeywordToken _:
                //    return ParseGetterFunction(attributes, modifiers);
                //case ISetKeywordToken _:
                //    return ParseSetterFunction(attributes, modifiers);
                case IVarKeywordToken _:
                case ILetKeywordToken _:
                    return ParseField(attributes, modifiers);
                //case IConstKeywordToken _:
                //    return ParseConst(attributes, modifiers);
                //default:
                //    return ParseIncompleteDeclaration(attributes, modifiers);
                case ICloseBraceToken _:
                    return null; // TODO clean up list parsing
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }

        #region Parse Namespaces
        [MustUseReturnValue]
        [NotNull]
        public NamespaceDeclarationSyntax ParseFileNamespace([NotNull] FixedList<string> name)
        {
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives(Tokens, Tokens.Context.Diagnostics).ToFixedList();
            var declarations = ParseDeclarations().ToFixedList();
            return new NamespaceDeclarationSyntax(true, name, null, usingDirectives, declarations);
        }

        [MustUseReturnValue]
        [NotNull]
        public NamespaceDeclarationSyntax ParseNamespaceDeclaration(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            // TODO generate errors for attributes or modifiers
            Tokens.Required<INamespaceKeywordToken>();
            var globalQualifier = Tokens.Accept<IColonColonToken>();
            var (name, span) = ParseNamespaceName();
            span = TextSpan.Covering(span, globalQualifier?.Span);
            Tokens.Expect<IOpenBraceToken>();
            var usingDirectives = usingDirectiveParser.ParseUsingDirectives(Tokens, Tokens.Context.Diagnostics).ToFixedList();
            var declarations = ParseDeclarations();
            Tokens.Expect<ICloseBraceToken>();
            return new NamespaceDeclarationSyntax(globalQualifier != null, name, span, usingDirectives, declarations);
        }

        [MustUseReturnValue]
        private (FixedList<string>, TextSpan) ParseNamespaceName()
        {
            var name = new List<string>();

            var nameSegment = Tokens.RequiredIdentifier();
            if (nameSegment == null)
                return (FixedList<string>.Empty, Tokens.Current.NotNull().Span);
            var span = nameSegment.Span;
            name.Add(nameSegment.Value);

            while (Tokens.Accept<IDotToken>() != null)
            {
                nameSegment = Tokens.ExpectIdentifier();
                if (nameSegment == null) break;
                span = TextSpan.Covering(span, nameSegment.Span);
                name.Add(nameSegment.Value);
            }

            return (name.ToFixedList(), span);
        }
        #endregion

        #region Parse Type Parts
        [MustUseReturnValue]
        [NotNull]
        private FixedList<AttributeSyntax> ParseAttributes()
        {
            var attributes = new List<AttributeSyntax>();
            // Take modifiers until null
            while (AcceptAttribute() is AttributeSyntax attribute)
                attributes.Add(attribute);
            return attributes.ToFixedList();
        }

        [MustUseReturnValue]
        [CanBeNull]
        private AttributeSyntax AcceptAttribute()
        {
            var hash = Tokens.Accept<IHashToken>();
            if (hash == null) return null;
            var name = nameParser.ParseName(Tokens, Tokens.Context.Diagnostics);
            var openParen = Tokens.Accept<IOpenParenToken>();
            SeparatedListSyntax<ArgumentSyntax> argumentList = null;
            ICloseParenTokenPlace closeParen = null;
            if (openParen != null)
            {
                argumentList = expressionParser.ParseArgumentList(Tokens, Tokens.Context.Diagnostics);
                closeParen = Tokens.Consume<ICloseParenTokenPlace>();
            }
            return new AttributeSyntax(name, argumentList);
        }

        //[MustUseReturnValue]
        //[NotNull]
        //private SyntaxList<ModifierSyntax> ParseModifiers(
        //    [NotNull] ITokenIterator tokens,
        //    [NotNull] Diagnostics diagnostics)
        //{
        //    var modifiers = new List<ModifierSyntax>();
        //    // Take modifiers until null
        //    while (modifierParser.AcceptModifier(tokens, diagnostics) is ModifierSyntax modifier)
        //        modifiers.Add(modifier);
        //    return new SyntaxList<ModifierSyntax>(modifiers);
        //}

        [MustUseReturnValue]
        [CanBeNull]
        private ExpressionSyntax AcceptBaseClass()
        {
            var colon = Tokens.Accept<IColonToken>();
            if (colon == null) return null;
            var typeExpression = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            return typeExpression;
        }

        [MustUseReturnValue]
        [CanBeNull]
        private FixedList<ExpressionSyntax> AcceptBaseTypes()
        {
            var lessThanColon = Tokens.Accept<ILessThanColonToken>();
            if (lessThanColon == null) return null;
            var typeExpressions = listParser.ParseSeparatedList<ExpressionSyntax, ICommaToken>(() => expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics));
            return typeExpressions;
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<InvariantSyntax> ParseInvariants()
        {
            return listParser.ParseList(AcceptInvariant);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private InvariantSyntax AcceptInvariant()
        {
            var invariantKeyword = Tokens.Accept<IInvariantKeywordToken>();
            if (invariantKeyword == null) return null;
            var condition = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            return new InvariantSyntax(invariantKeyword, condition);
        }
        #endregion

        #region Parse Type Declarations
        [MustUseReturnValue]
        [NotNull]
        private ClassDeclarationSyntax ParseClass(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var classKeyword = Tokens.Required<IClassKeywordToken>();
            var name = Tokens.ExpectIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            var baseClass = AcceptBaseClass();
            var baseTypes = AcceptBaseTypes();
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var invariants = ParseInvariants();
            Tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(ParseMemberDeclaration);
            Tokens.Expect<ICloseBraceToken>();
            return new ClassDeclarationSyntax(attributes, modifiers, name, genericParameters,
                baseClass, baseTypes, genericConstraints, invariants,
                members);
        }

        [MustUseReturnValue]
        [NotNull]
        private TraitDeclarationSyntax ParseTrait(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var typeKeyword = Tokens.Required<ITraitKeywordToken>();
            var name = Tokens.ExpectIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            var baseTypes = AcceptBaseTypes();
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var invariants = ParseInvariants();
            Tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(ParseMemberDeclaration);
            Tokens.Expect<ICloseBraceToken>();
            return new TraitDeclarationSyntax(attributes, modifiers, name,
                genericParameters, baseTypes, genericConstraints, invariants,
                members);
        }

        [MustUseReturnValue]
        [NotNull]
        private StructDeclarationSyntax ParseStruct(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Required<IStructKeywordToken>();
            var name = Tokens.RequiredIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            var baseTypes = AcceptBaseTypes();
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var invariants = ParseInvariants();
            Tokens.Expect<IOpenBraceToken>();
            var members = listParser.ParseList(ParseMemberDeclaration);
            Tokens.Expect<ICloseBraceToken>();
            return new StructDeclarationSyntax(attributes, modifiers, name, genericParameters,
                baseTypes, genericConstraints, invariants,
                members);
        }

        [MustUseReturnValue]
        [NotNull]
        private MemberDeclarationSyntax ParseEnum(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var enumKeyword = Tokens.Required<IEnumKeywordToken>();
            switch (Tokens.Current)
            {
                case IStructKeywordToken _:
                {
                    var structKeyword = Tokens.Expect<IStructKeywordToken>();
                    var name = Tokens.ExpectIdentifier();
                    var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
                    var baseTypes = AcceptBaseTypes();
                    var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
                    var invariants = ParseInvariants();
                    Tokens.Expect<IOpenBraceToken>();
                    var variants = ParseEnumVariants();
                    var members = listParser.ParseList(ParseMemberDeclaration);
                    Tokens.Expect<ICloseBraceToken>();
                    return new EnumStructDeclarationSyntax(attributes, modifiers,
                        name, genericParameters, baseTypes, genericConstraints,
                        invariants, variants, members);
                }
                case IClassKeywordToken _:
                {
                    var classKeyword = Tokens.Expect<IClassKeywordToken>();
                    var name = Tokens.ExpectIdentifier();
                    var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
                    var baseClass = AcceptBaseClass();
                    var baseTypes = AcceptBaseTypes();
                    var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
                    var invariants = ParseInvariants();
                    Tokens.Expect<IOpenBraceToken>();
                    var variants = ParseEnumVariants();
                    var members = listParser.ParseList(ParseMemberDeclaration);
                    Tokens.Expect<ICloseBraceToken>();
                    return new EnumClassDeclarationSyntax(attributes, modifiers,
                        name, genericParameters, baseClass, baseTypes, genericConstraints,
                        invariants, variants, members);
                }
                default:
                    throw new ParseFailedException();
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private EnumVariantsSyntax ParseEnumVariants()
        {
            var variants = new List<EnumVariantSyntax>();
            while (Tokens.Current is IIdentifierToken)
                variants.Add(ParseEnumVariant());

            ISemicolonTokenPlace semicolon = null;
            if (!(Tokens.Current is ICloseBraceToken))
                semicolon = Tokens.Consume<ISemicolonTokenPlace>();

            return new EnumVariantsSyntax(variants.ToFixedList(), semicolon);
        }

        [MustUseReturnValue]
        [NotNull]
        private EnumVariantSyntax ParseEnumVariant()
        {
            var identifier = Tokens.ExpectIdentifier();
            var comma = Tokens.Accept<ICommaToken>();
            return new EnumVariantSyntax(identifier, comma);
        }
        #endregion

        #region Parse Type Member Declarations
        [MustUseReturnValue]
        [NotNull]
        private FieldDeclarationSyntax ParseField(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var binding = Tokens.Required<IBindingToken>();
            var getter = AcceptFieldGetter();
            var name = Tokens.ExpectIdentifier();
            ExpressionSyntax typeExpression = null;
            if (Tokens.Current is IColonToken)
            {
                Tokens.Expect<IColonToken>();
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                typeExpression = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics, OperatorPrecedence.AboveAssignment);
            }
            ExpressionSyntax initializer = null;
            if (Tokens.Current is IEqualsToken)
            {
                Tokens.Consume<IEqualsToken>();
                initializer = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            }
            Tokens.Expect<ISemicolonToken>();
            return new FieldDeclarationSyntax(modifiers, name, typeExpression, initializer);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private AccessModifier? AcceptFieldGetter()
        {
            // TODO allow other access modifiers
            var publicKeyword = Tokens.Accept<IPublicKeywordToken>();
            var getKeyword = Tokens.Accept<IGetKeywordToken>();
            if (publicKeyword == null && getKeyword == null) return null;
            return AccessModifier.Public;
        }
        #endregion

        #region Parse Functions
        [MustUseReturnValue]
        [NotNull]
        public NamedFunctionDeclarationSyntax ParseNamedFunction(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            Tokens.Required<IFunctionKeywordToken>();
            var name = Tokens.RequiredIdentifier();
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            var parameters = AcceptParameters();
            Tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = ParseFunctionBody(Tokens, Tokens.Context.Diagnostics);
            return new NamedFunctionDeclarationSyntax(modifiers, name, genericParameters, parameters, returnTypeExpression,
                genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public OperatorDeclarationSyntax ParseOperatorFunction(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var operatorKeywordSpan = Tokens.Required<IOperatorKeywordToken>();
            // TODO save the generic parameters
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            //IOperatorTokenPlace @operator;
            // TODO correctly store these in the syntax class
            switch (Tokens.Current)
            {
                case IHashToken _:
                    Tokens.Expect<IHashToken>();
                    switch (Tokens.Current)
                    {
                        case IOpenParenToken _:
                            Tokens.Next();
                            Tokens.Expect<ICloseParenToken>();
                            break;
                        case IOpenBracketToken _:
                            Tokens.Next();
                            Tokens.Expect<ICloseBracketToken>();
                            break;
                        case IOpenBraceToken _:
                            Tokens.Next();
                            Tokens.Expect<ICloseBraceToken>();
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(Tokens.Current);
                    }
                    break;
                case IStringLiteralToken _:
                    Tokens.Expect<IStringLiteralToken>();
                    // TODO need to check it is empty string
                    break;
                // TODO case for user defined literals ''
                default:
                    Tokens.Expect<IOperatorToken>();
                    break;
            }
            var parameters = AcceptParameters();
            Tokens.Expect<IRightArrowToken>();
            var returnTypeExpression = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = blockParser.ParseBlock(Tokens, Tokens.Context.Diagnostics);
            return new OperatorDeclarationSyntax(modifiers, operatorKeywordSpan, genericParameters,
                 parameters, returnTypeExpression,
                genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public ConstructorDeclarationSyntax ParseConstructor(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var newKeywordSpan = Tokens.Required<INewKeywordToken>();
            var name = Tokens.Accept<IIdentifierToken>();
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            var parameters = AcceptParameters();
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = blockParser.ParseBlock(Tokens, Tokens.Context.Diagnostics);
            return new ConstructorDeclarationSyntax(modifiers, name, TextSpan.Covering(newKeywordSpan, name?.Span),
                genericParameters, parameters, genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }

        [MustUseReturnValue]
        [NotNull]
        public InitializerDeclarationSyntax ParseInitializer(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var initKeywordSpan = Tokens.Required<IInitKeywordToken>();
            var name = Tokens.Accept<IIdentifierToken>();
            var genericParameters = genericsParser.AcceptGenericParameters(Tokens, Tokens.Context.Diagnostics);
            var parameters = AcceptParameters();
            var genericConstraints = genericsParser.ParseGenericConstraints(Tokens, Tokens.Context.Diagnostics);
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = blockParser.ParseBlock(Tokens, Tokens.Context.Diagnostics);
            return new InitializerDeclarationSyntax(modifiers, name, TextSpan.Covering(initKeywordSpan, name?.Span),
                genericParameters, parameters, genericConstraints, mayEffects, noEffects, requires, ensures, body);
        }


        [MustUseReturnValue]
        [NotNull]
        public DestructorDeclarationSyntax ParseDestructor(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers)
        {
            var deleteKeywordSpan = Tokens.Required<IDeleteKeywordToken>();
            var parameters = AcceptParameters();
            var mayEffects = ParseMayEffects();
            var noEffects = ParseNoEffects();
            var (requires, ensures) = ParseFunctionContracts();
            var body = blockParser.ParseBlock(Tokens, Tokens.Context.Diagnostics);
            return new DestructorDeclarationSyntax(modifiers, deleteKeywordSpan,
                 parameters, mayEffects, noEffects, requires, ensures, body);
        }

        //[MustUseReturnValue]
        //[NotNull]
        //private GetterFunctionDeclarationSyntax ParseGetterFunction(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var getKeyword = tokens.Take<IGetKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var arrow = tokens.Expect<IRightArrowTokenPlace>();
        //    var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
        //    return new GetterFunctionDeclarationSyntax(modifiers, getKeyword, name,
        //        openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private SetterFunctionDeclarationSyntax ParseSetterFunction(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var setKeyword = tokens.Take<ISetKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    var openParen = tokens.Expect<IOpenParenTokenPlace>();
        //    var parameters = ParseParameterList(tokens, diagnostics);
        //    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
        //    var arrow = tokens.Expect<IRightArrowTokenPlace>();
        //    var returnTypeExpression = expressionParser.ParseExpression(tokens, diagnostics);
        //    var effects = AcceptEffects(tokens, diagnostics);
        //    var contracts = ParseFunctionContracts(tokens, diagnostics);
        //    var (body, semicolon) = ParseFunctionBody(tokens, diagnostics);
        //    return new SetterFunctionDeclarationSyntax(modifiers, setKeyword, name,
        //        openParen, parameters, closeParen, arrow, returnTypeExpression, effects, contracts, body, semicolon);
        //}

        [MustUseReturnValue]
        [CanBeNull]
        private FixedList<ParameterSyntax> AcceptParameters()
        {
            var parameters = new List<ParameterSyntax>();
            if (Tokens.Accept<IOpenParenToken>() == null)
                return null;
            while (!Tokens.AtEndOfFile() && !(Tokens.Current is ICloseParenToken))
            {
                parameters.Add(parameterParser.ParseParameter(Tokens, Tokens.Context.Diagnostics));
                if (Tokens.Current is ICommaToken)
                    Tokens.Expect<ICommaToken>();
            }
            Tokens.Expect<ICloseParenToken>();
            return parameters.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<EffectSyntax> ParseMayEffects()
        {
            if (Tokens.Accept<IMayKeywordToken>() == null)
                return FixedList<EffectSyntax>.Empty;

            return listParser.ParseSeparatedList<EffectSyntax, ICommaToken>(ParseEffect);
        }

        [MustUseReturnValue]
        [NotNull]
        private FixedList<EffectSyntax> ParseNoEffects()
        {
            if (Tokens.Accept<INoKeywordToken>() == null)
                return FixedList<EffectSyntax>.Empty;

            return listParser.ParseSeparatedList<EffectSyntax, ICommaToken>(ParseEffect);
        }


        [MustUseReturnValue]
        [NotNull]
        public EffectSyntax ParseEffect()
        {
            switch (Tokens.Current)
            {
                case IThrowKeywordToken throwKeyword:
                    Tokens.Next();
                    var exceptions = listParser.ParseSeparatedList<ThrowEffectEntrySyntax, ICommaToken>(ParseThrowEffectEntry);
                    return new ThrowEffectSyntax(exceptions);
                case IIdentifierToken identifier:
                    Tokens.Next();
                    return new SimpleEffectSyntax(identifier);
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ThrowEffectEntrySyntax ParseThrowEffectEntry()
        {
            var paramsKeyword = Tokens.Accept<IParamsKeywordToken>();
            var exceptionType = expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
            return new ThrowEffectEntrySyntax(paramsKeyword, exceptionType);
        }

        [MustUseReturnValue]
        [NotNull]
        private (FixedList<ExpressionSyntax> Requires, FixedList<ExpressionSyntax> Ensures) ParseFunctionContracts()
        {
            var requires = new List<ExpressionSyntax>();
            var ensures = new List<ExpressionSyntax>();
            for (; ; )
                switch (Tokens.Current)
                {
                    case IRequiresKeywordToken _:
                        requires.Add(ParseRequires());
                        break;
                    case IEnsuresKeywordToken _:
                        ensures.Add(ParseEnsures());
                        break;
                    default:
                        return (requires.ToFixedList(), ensures.ToFixedList());
                }
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseRequires()
        {
            Tokens.Required<IRequiresKeywordToken>();
            return expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseEnsures()
        {
            Tokens.Required<IEnsuresKeywordToken>();
            return expressionParser.ParseExpression(Tokens, Tokens.Context.Diagnostics);
        }

        private BlockSyntax ParseFunctionBody(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var body = blockParser.AcceptBlock(tokens, diagnostics);
            if (body == null)
                tokens.Expect<ISemicolonToken>();
            return body;
        }
        #endregion

        //[MustUseReturnValue]
        //[NotNull]
        //private ConstDeclarationSyntax ParseConst(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var constKeyword = tokens.Take<IConstKeywordToken>();
        //    var name = tokens.ExpectIdentifier();
        //    IColonTokenPlace colon = null;
        //    ExpressionSyntax typeExpression = null;
        //    if (tokens.Current is IColonToken)
        //    {
        //        colon = tokens.Expect<IColonTokenPlace>();
        //        // Need to not consume the assignment that separates the type from the initializer,
        //        // hence the min operator precedence.
        //        typeExpression = expressionParser.ParseExpression(tokens, diagnostics, OperatorPrecedence.AboveAssignment);
        //    }
        //    IEqualsToken equals = null;
        //    ExpressionSyntax initializer = null;
        //    if (tokens.Current is IEqualsToken)
        //    {
        //        equals = tokens.Take<IEqualsToken>();
        //        initializer = expressionParser.ParseExpression(tokens, diagnostics);
        //    }
        //    var semicolon = tokens.Expect<ISemicolonTokenPlace>();
        //    return new ConstDeclarationSyntax(attributes, modifiers, constKeyword, name, colon, typeExpression,
        //        equals, initializer, semicolon);
        //}

        //[MustUseReturnValue]
        //[NotNull]
        //private static IncompleteDeclarationSyntax ParseIncompleteDeclaration(
        //    [NotNull] FixedList<AttributeSyntax> attributes,
        //    [NotNull] FixedList<IModiferToken> modifiers)
        //{
        //    var skipped = attributes.SelectMany(a => a.Tokens()).Concat(modifiers.SelectMany(m => m.Tokens())).ToList();
        //    skipped.Add(tokens.ExpectIdentifier());    // The name we are expecting

        //    if (skipped.All(s => s is IMissingToken))
        //    {
        //        // We haven't consumed any tokens, we need to consume a token so
        //        // we make progress.
        //        skipped.Add(tokens.Current);
        //        tokens.Next();
        //    }

        //    var span = TextSpan.Covering(
        //        skipped.First(s => s != null).NotNull().Span,
        //        skipped.Last(s => s != null).NotNull().Span);

        //    diagnostics.Add(ParseError.IncompleteDeclaration(tokens.Context.File, span));
        //    return new IncompleteDeclarationSyntax(skipped);
        //}
    }
}
