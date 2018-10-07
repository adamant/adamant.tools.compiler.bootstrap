using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tests
{
    public class CustomType
    {
        public const string MagicValue = "7hf78hd7wefhwefh88";
        public readonly string Value;

        public CustomType(string value)
        {
            Value = value;
        }
    }

    public class Example
    {
        static Example()
        {
            Arb.Register<Example>();
        }

        public static Arbitrary<CustomType> ArbitraryCustomType()
        {
            return Arb.From(Gen.Constant(new CustomType(CustomType.MagicValue)));
        }

        [Property(MaxTest = 1, Replay = "445409081,296507319")]
        public bool ProblemTest(CustomType c)
        {
            return CustomType.MagicValue == c.Value;
        }

        // If this test is removed or is [Fact] the problem goes away
        [Theory]
        [InlineData("value")]
        public void PassingTest(string _)
        {
        }
    }
}
