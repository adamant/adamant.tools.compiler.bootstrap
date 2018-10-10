using System;
using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Tests.Data
{
    public static class ProjectDirectory
    {
        public static string Get()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var workDirectory = currentDirectory;
            var root = Path.GetPathRoot(currentDirectory);
            while (workDirectory != root)
            {
                if (Path.GetFileName(workDirectory) == "Semantics.Tests")
                    return workDirectory;

                workDirectory = Path.GetDirectoryName(workDirectory);
            }

            throw new Exception($"Could not find \"Semantics.Tests\" in {currentDirectory}");
        }
    }
}
