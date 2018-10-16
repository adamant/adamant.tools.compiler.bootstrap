using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics
{
    [UnitTest]
    [Category("Analyze")]
    public class DeclarationBuilderSpec
    {
        [Fact]
        public void Ignores_incomplete_declarations()
        {
            var incompleteDeclaration = FakeSyntax.IncompleteDeclaration();
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(incompleteDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);
            var package = new Package("Test");

            GatherDeclarations(package, packageSyntax);

            Assert.Empty(package.Declarations);
        }

        [Fact]
        public void Function_without_namespace()
        {
            var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(functionDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);
            var package = new Package("Test");

            GatherDeclarations(package, packageSyntax);

            Assert.Collection(package.Declarations, d =>
            {
                var f = Assert.IsType<FunctionDeclaration>(d);
                Assert.Equal("function_name", f.QualifiedName.ToString());
            });
        }

        [Fact]
        public void Function_with_namespace()
        {
            var functionDeclaration = FakeSyntax.FunctionDeclaration("function_name");
            var @namespace = FakeSyntax.Name("myNamespace.name");
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(@namespace, functionDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);
            var package = new Package("Test");

            GatherDeclarations(package, packageSyntax);

            Assert.Collection(package.Declarations, d =>
            {
                var f = Assert.IsType<FunctionDeclaration>(d);
                Assert.Equal("myNamespace.name.function_name", f.QualifiedName.ToString());
            });
        }

        [Fact]
        public void Enum_struct()
        {
            var functionDeclaration = FakeSyntax.EnumStructDeclaration("My_Struct");
            var compilationUnitSyntax = FakeSyntax.CompilationUnit(functionDeclaration);
            var packageSyntax = FakeSyntax.Package(compilationUnitSyntax);
            var package = new Package("Test");

            GatherDeclarations(package, packageSyntax);

            Assert.Collection(package.Declarations, d =>
            {
                var f = Assert.IsType<TypeDeclaration>(d);
                Assert.Equal("My_Struct", f.QualifiedName.ToString());
            });
        }

        private static void GatherDeclarations(
            [NotNull] Package package,
            [NotNull] PackageSyntax packageSyntax)
        {
            new DeclarationBuilder(new NameBuilder()).GatherDeclarations(package, packageSyntax);
        }
    }
}
