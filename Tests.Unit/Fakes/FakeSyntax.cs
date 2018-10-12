using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    // A factory for fakes
    public static class FakeSyntax
    {
        [NotNull]
        public static AccessModifierSyntax AccessModifier()
        {
            return new AccessModifierSyntax(null);
        }

        [NotNull]
        public static BlockStatementSyntax BlockStatement()
        {
            return new BlockStatementSyntax(null, List<StatementSyntax>(), null);
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
                    nameSyntax = new QualifiedNameSyntax(nameSyntax, null, partSyntax);
            }

            return nameSyntax;
        }

        [NotNull]
        public static UsingDirectiveSyntax UsingDirective()
        {
            return new UsingDirectiveSyntax(null, Name(), null);
        }

        public static ParameterSyntax Parameter()
        {
            return new ParameterSyntax(null, null, null, FakeSyntax.Expression());
        }

        private class FakeExpressionSyntax : ExpressionSyntax
        {
        }

        private class FakeNameSyntax : NameSyntax
        {
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
                new EndOfFileToken(new TextSpan(0, 0), Diagnostics.Empty),
                Diagnostics.Empty);
        }

        [NotNull]
        public static PackageSyntax Package([NotNull]params CompilationUnitSyntax[] compilationUnits)
        {
            return new PackageSyntax("Test", new SyntaxList<CompilationUnitSyntax>(compilationUnits));
        }

        [NotNull]
        public static FunctionDeclarationSyntax FunctionDeclaration([NotNull] string name)
        {
            return new FunctionDeclarationSyntax(
                AccessModifier(),
                null,
                Identifier(name),
                null,
                SeparatedListSyntax<ParameterSyntax>.Empty,
                null,
                null,
                Expression(),
                BlockStatement());
        }

        [NotNull]
        public static IdentifierToken Identifier(string name)
        {
            return new BareIdentifierToken(new TextSpan(0, 0), name);
        }

        public static EnumStructDeclarationSyntax EnumStructDeclaration(string name)
        {
            return new EnumStructDeclarationSyntax(
                AccessModifier(),
                null,
                null,
                Identifier(name),
                null,
                SyntaxList<MemberDeclarationSyntax>.Empty,
                null);
        }
    }
}
