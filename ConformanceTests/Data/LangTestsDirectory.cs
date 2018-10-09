using System;
using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.ConformanceTests.Data
{
    public static class LangTestsDirectory
    {
        public static string Get()
        {
            const string dirName = "ConformanceTests";

            var currentDirectory = Directory.GetCurrentDirectory();
            var workDirectory = currentDirectory;
            var root = Path.GetPathRoot(currentDirectory);
            while (workDirectory != root)
            {

                if (Path.GetFileName(workDirectory) == dirName)
                    return Path.Combine(Path.GetDirectoryName(workDirectory), "tests");

                if (Directory.Exists(Path.Combine(workDirectory, dirName)))
                {
                    return Path.Combine(workDirectory, "tests");
                }

                workDirectory = Path.GetDirectoryName(workDirectory);
            }

            throw new Exception($"Could not find \"{dirName}\" from {currentDirectory}");
        }
    }
}
