using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C
{
    [UnitTest]
    [Category("Emit")]
    public class ResourcesSpec
    {

        private readonly IReadOnlyList<byte> byteOrderMarkUtf8 = Encoding.UTF8.GetPreamble().ToList().AsReadOnly();

        /// <summary>
        /// In order for .NET to read the resources files as UTF-8 they must have a BOM. Otherwise,
        /// they are read with some other encoding and all non-ascii characters are messed up.
        /// </summary>
        [Theory]
        [InlineData(CodeEmitter.RuntimeLibraryCodeFileName)]
        [InlineData(CodeEmitter.RuntimeLibraryHeaderFileName)]
        public void Resources_start_with_BOM(string filename)
        {
            var projectDir = Path.Combine(SolutionDirectory.Get(), "Emit.C");

            var codeFileBytes = File.ReadAllBytes(Path.Combine(projectDir, filename));
            AssertStartsWithUtf8Bom(codeFileBytes);
        }

        private void AssertStartsWithUtf8Bom(byte[] bytes)
        {
            Assert.True(bytes.Length >= byteOrderMarkUtf8.Count, $"Length = {bytes.Length}");
            for (var i = 0; i < byteOrderMarkUtf8.Count; i++)
                Assert.Equal(byteOrderMarkUtf8[i], bytes[i]);
        }
    }
}
