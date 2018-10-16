using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics
{
    [UnitTest]
    [Category("Analyze")]
    public class AnalysisBuilderSpec
    {
        [Fact]
        public void Ignores_incomplete_declarations()
        {
            var incompleteDeclaration = FakeSyntax.IncompleteDeclaration();
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(incompleteDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);
            var package = new Package("Test");

            var (scopes, analyses) = PrepareForAnalysis(packageSyntax);

            Assert.Empty(analyses);
        }

        [Fact]
        public void Function_without_namespace()
        {
            var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(functionDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);

            var (scopes, analyses) = PrepareForAnalysis(packageSyntax);

            Assert.Collection(analyses, a =>
            {
                var f = Assert.IsType<FunctionAnalysis>(a);
                Assert.Equal("function_name", f.Semantics.QualifiedName.ToString());
            });
        }

        [Fact]
        public void Function_with_namespace()
        {
            var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");
            var @namespace = FakeSyntax.Name("myNamespace.name");
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(@namespace, functionDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);

            var (scopes, analyses) = PrepareForAnalysis(packageSyntax);

            Assert.Collection(analyses, a =>
            {
                var f = Assert.IsType<FunctionAnalysis>(a);
                Assert.Equal("myNamespace.name.function_name", f.Semantics.QualifiedName.ToString());
            });
        }

        [Fact]
        public void Enum_struct()
        {
            var functionDeclaration = FakeSyntax.EnumStructDeclaration("My_Struct");
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(functionDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);

            var (scopes, analyses) = PrepareForAnalysis(packageSyntax);

            Assert.Collection(analyses, a =>
            {
                var t = Assert.IsType<TypeAnalysis>(a);
                Assert.Equal("My_Struct", t.Semantics.QualifiedName.ToString());
            });
        }

        private static (IList<CompilationUnitScope>, IList<DeclarationAnalysis>) PrepareForAnalysis(
            [NotNull] PackageSyntax packageSyntax)
        {
            return new AnalysisBuilder(new NameBuilder()).PrepareForAnalysis(packageSyntax);
        }
    }
}
