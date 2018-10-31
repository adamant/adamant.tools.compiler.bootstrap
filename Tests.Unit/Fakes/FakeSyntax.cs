using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Enums;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
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
            where T : SyntaxNode
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
            where T : SyntaxNode
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
            return new NamedParameterSyntax(null, null, Missing<IIdentifierToken>(), Missing<IColonToken>(), Expression());
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
            return new IncompleteDeclarationSyntax(Enumerable.Empty<Token>());
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
            var namespaceSyntax = @namespace != null
                ? new CompilationUnitNamespaceSyntax(null, @namespace, null)
                : null;
            return new CompilationUnitSyntax(
                "".ToFakeCodeFile(),
                namespaceSyntax,
                SyntaxList<UsingDirectiveSyntax>.Empty,
                new SyntaxList<DeclarationSyntax>(declarations),
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
                null, // Effects
                SyntaxList<ContractSyntax>.Empty,
                Block(),
                null); // Semicolon
        }


        [NotNull]
        public static IdentifierToken Identifier(string name)
        {
            return new BareIdentifierToken(FakeSpan, name);
        }

        [NotNull]
        public static EnumStructDeclarationSyntax EnumStructDeclaration(string name)
        {
            return new EnumStructDeclarationSyntax(
                AccessModifier().ToSyntaxList<ModifierSyntax>(),
                new EnumKeywordToken(FakeSpan),
                Missing<IStructKeywordToken>(),
                Identifier(name),
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
