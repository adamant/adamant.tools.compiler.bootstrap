using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.UnitTests.Semantics
{
    [UnitTest]
    [Category("Analyze")]
    public class DeclarationBuilderSpec
    {
        [Fact]
        public void Ignores_incomplete_declarations()
        {
            //var compilationUnitSyntax = new CompilationUnitSyntax();
            //var packageSyntax = new PackageSyntax("test",)
            //var package = new Package();

            //new DeclarationBuilder().GatherDeclarations();
        }
    }
}
