using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers
{
    public static class SolutionDirectory
    {
        public static string Get()
        {
            var directory = Directory.GetCurrentDirectory();
            while (directory != null && !Directory.GetFiles(directory, "*.sln", SearchOption.TopDirectoryOnly).Any())
            {
                directory = Path.GetDirectoryName(directory);
            }
            return directory;
        }
    }
}
