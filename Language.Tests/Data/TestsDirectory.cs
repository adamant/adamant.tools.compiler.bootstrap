using System;
using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public static class TestsDirectory
    {
        public static string Get()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var workDirectory = currentDirectory;
            var root = Path.GetPathRoot(currentDirectory);
            while (workDirectory != root)
            {
                if (Path.GetFileName(workDirectory) == "Language.Tests")
                    return Path.Combine(Path.GetDirectoryName(workDirectory), "tests");

                workDirectory = Path.GetDirectoryName(workDirectory);
            }

            throw new Exception($"Could not find \"Language.Tests\" in {currentDirectory}");
        }
    }
}
