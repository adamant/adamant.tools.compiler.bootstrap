using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;
using static Adamant.Tools.Compiler.Bootstrap.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class ReferenceCapabilityTests
    {
        [Fact]
        public void Source_code_string_of_shared_is_empty()
        {
            var sourceCode = Shared.ToSourceCodeString();

            Assert.Equal("", sourceCode);
        }

        /// <summary>
        /// It is frequently confusing that shared has no textual keyword. In
        /// IL, we make sharing it explicit.
        /// </summary>
        [Fact]
        public void IL_string_of_shared_is_empty()
        {
            var sourceCode = Shared.ToILString();

            Assert.Equal("shared", sourceCode);
        }
    }
}
