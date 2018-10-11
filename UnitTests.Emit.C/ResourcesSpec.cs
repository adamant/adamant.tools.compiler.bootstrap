using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Emit.C
{
    public class ResourcesSpec
    {
        [NotNull]
        private readonly string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        /// <summary>
        /// In order for .NET to read the resources files as UTF-8 they must have a BOM. Otherwise,
        /// they are read with some other encoding and all non-ascii characters are messed up.
        /// </summary>
        [Fact]
        public void ResourcesStartWithBOM()
        {
            Assert.StartsWith(byteOrderMarkUtf8, CodeEmitter.RuntimeLibraryCode);
            Assert.StartsWith(byteOrderMarkUtf8, CodeEmitter.RuntimeLibraryHeader);
        }
    }
}
