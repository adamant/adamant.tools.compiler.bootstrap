using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    // A factory for fakes
    public static class FakeSyntax
    {
        private static readonly TextSpan FakeSpan = new TextSpan(0, 0);

        [NotNull]
        private static T Missing<T>()
            where T : IToken
        {
            return (T)(object)new MissingToken(FakeSpan);
        }

        [NotNull]
        public static AccessModifierSyntax AccessModifier()
        {
            return new AccessModifierSyntax(Missing<IAccessModifierToken>());
        }

        [NotNull]
        public static BlockSyntax Block()
        {
            return new BlockSyntax(Missing<IOpenBraceToken>(), List<StatementSyntax>(), Missing<ICloseBraceToken>());
        }

        [NotNull]
        public static SyntaxList<T> List<T>()
            where T : NonTerminal
        {
            return new SyntaxList<T>(Enumerable.Empty<T>());
        }

        [NotNull]
        public static ExpressionSyntax Expression()
        {
            return new FakeExpressionSyntax();
        }

        [NotNull]
        public static SeparatedListSyntax<T> SeparatedList<T>()
            where T : NonTerminal
        {
            return new SeparatedListSyntax<T>(Enumerable.Empty<T>());
        }

        [NotNull]
        public static NameSyntax Name()
        {
            return new FakeNameSyntax();
        }

        [NotNull]
        public static NameSyntax Name([NotNull] string name)
        {
            var parts = name.Split('.').AssertNotNull();

            NameSyntax nameSyntax = null;
            foreach (var part in parts)
            {
                var partSyntax = new IdentifierNameSyntax(Identifier(part));
                if (nameSyntax == null)
                    nameSyntax = partSyntax;
                else
                    nameSyntax = new QualifiedNameSyntax(nameSyntax, new DotToken(new TextSpan(0, 1)), partSyntax);
            }

            return nameSyntax.AssertNotNull();
        }

        [NotNull]
        public static UsingDirectiveSyntax UsingDirective()
        {
            return new UsingDirectiveSyntax(Missing<IUsingKeywordToken>(), Name(), Missing<ISemicolonToken>());
        }

        public static ParameterSyntax Parameter()
        {
            return new NamedParameterSyntax(null, null, Missing<IIdentifierToken>(),
                Missing<IColonToken>(), Expression(), null, null);
        }

        private class FakeExpressionSyntax : ExpressionSyntax
        {
            public FakeExpressionSyntax()
                : base(FakeSpan)
            {
            }
        }

        private class FakeNameSyntax : NameSyntax
        {
            public FakeNameSyntax()
                : base(FakeSpan)
            {
            }
        }

        [NotNull]
        public static IncompleteDeclarationSyntax IncompleteDeclaration()
        {
            // Need at least one token to prevent issues computing the span of the incomplete declaration
            return new IncompleteDeclarationSyntax(new[] { Missing<IPublicKeywordToken>() });
        }

        [NotNull]
        public static CompilationUnitSyntax CompilationUnit(
            [NotNull] params DeclarationSyntax[] declarations)
        {
            return CompilationUnit(null, declarations);
        }

        [NotNull]
        public static CompilationUnitSyntax CompilationUnit(
            [CanBeNull] NameSyntax @namespace,
            [NotNull] params DeclarationSyntax[] declarations)
        {
            var namespaceSyntax = new FileNamespaceDeclarationSyntax(
                null, @namespace, null, SyntaxList<UsingDirectiveSyntax>.Empty,
                new SyntaxList<DeclarationSyntax>(declarations)); ;
            return new CompilationUnitSyntax(
                "".ToFakeCodeFile(),
                namespaceSyntax,
                new EndOfFileToken(FakeSpan, Diagnostics.Empty),
                Diagnostics.Empty);
        }

        [NotNull]
        public static PackageSyntax Package([NotNull]params CompilationUnitSyntax[] compilationUnits)
        {
            return new PackageSyntax("Test", new SyntaxList<CompilationUnitSyntax>(compilationUnits));
        }

        [NotNull]
        public static NamedFunctionDeclarationSyntax FunctionDeclaration([NotNull] string name)
        {
            return new NamedFunctionDeclarationSyntax(
                AccessModifier().ToSyntaxList<ModifierSyntax>(),
                new FunctionKeywordToken(FakeSpan),
                Identifier(name),
                null, // Generic Parameters
                Missing<IOpenParenToken>(),
                SeparatedListSyntax<ParameterSyntax>.Empty,
                Missing<ICloseParenToken>(),
                Missing<IRightArrowToken>(),
                Expression(),
                SyntaxList<GenericConstraintSyntax>.Empty,
                null, // Effects
                SyntaxList<FunctionContractSyntax>.Empty,
                Block(),
                null); // Semicolon
        }


        [NotNull]
        public static IdentifierToken Identifier([NotNull] string name)
        {
            return new BareIdentifierToken(FakeSpan, name);
        }

        [NotNull]
        public static EnumStructDeclarationSyntax EnumStructDeclaration([NotNull] string name)
        {
            return new EnumStructDeclarationSyntax(
                AccessModifier().ToSyntaxList<ModifierSyntax>(),
                new EnumKeywordToken(FakeSpan),
                new StructKeywordToken(FakeSpan),
                Identifier(name),
                null, // Generic Parameters
                null, // Base Types
                SyntaxList<GenericConstraintSyntax>.Empty,
                SyntaxList<InvariantSyntax>.Empty,
                Missing<IOpenBraceToken>(),
                new EnumVariantsSyntax(SyntaxList<EnumVariantSyntax>.Empty, null),
                SyntaxList<MemberDeclarationSyntax>.Empty,
                Missing<ICloseBraceToken>());
        }

        [NotNull]
        public static GenericParametersSyntax GenericParameters()
        {
            return new GenericParametersSyntax(
                Missing<IOpenBracketToken>(),
                SeparatedListSyntax<GenericParameterSyntax>.Empty,
                Missing<ICloseBracketToken>());
        }
    }
}
