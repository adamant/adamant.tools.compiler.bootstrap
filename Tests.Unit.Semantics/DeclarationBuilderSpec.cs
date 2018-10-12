using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
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

            new DeclarationBuilder().GatherDeclarations(package, packageSyntax);

            Assert.Empty(package.Declarations);
        }
    }
}
