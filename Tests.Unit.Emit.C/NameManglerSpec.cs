using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C
{
    [UnitTest]
    [Category("Emit")]
    public class NameManglerSpec
    {
        [Theory]
        // Basic Multilingual Plane
        [InlineData("x", "x")]
        [InlineData("My_Class", "My_Class")]
        [InlineData("class", "class")] // Keywords aren't special
        [InlineData("_059amzAMZ", "_059amzAMZ")] // Basic ASCII range
        [InlineData("Hello World", "Hello_World")]
        //[InlineData("do_it!", "do_itµ21ǂ")] // Encode other chars
        //[InlineData("ᵢₐ˽·´µǂ", "µ1D62ǂµ2090ǂµ2FDǂµB7ǂµB4ǂµB5ǂµ1C2ǂ")] // Encode chars used for escaping
        //[InlineData("\xF8\u1681\uD7FF\uF900\uFFFD", "\xF8\u1681\uD7FF\uF900\uFFFD")] // Already Valid
        //[InlineData("\x0F\xAB\u1680", "µFǂµABǂµ1680ǂ")] // Not Valid, all lengths in BMP
        // Supplementary Planes
        //[InlineData("\U00010000\U0003FFFD\U0009F1FD", "\U00010000\U0003FFFD\U0009F1FD")] // Already Valid
        //[InlineData("\U0001FFFE\U000F1234\U0010FFFF", "µ1FFFEǂµF1234ǂµ10FFFFǂ")] // Not Valid, all lengths
        public void Mangle_part([NotNull] string name, [NotNull] string expectedMangledName)
        {
            var builder = new StringBuilder();

            NameMangler.ManglePart(name, builder);

            Assert.Equal(expectedMangledName, builder.ToString());
        }
    }
}
