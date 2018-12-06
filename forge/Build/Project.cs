using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Forge.Config;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Build
{
    internal class Project
    {
        public string Path { get; }
        public FixedList<string> RootNamespace { get; }
        public string Name { get; }
        public FixedList<string> Authors { get; }
        public ProjectTemplate Template { get; }
        public FixedList<ProjectReference> References { get; }

        public Project(
            ProjectConfig file,
            IEnumerable<ProjectReference> references)
        {
            Path = System.IO.Path.GetDirectoryName(file.FullPath);
            Name = file.Name ?? throw new InvalidOperationException();
            RootNamespace = (file.RootNamespace ?? "").SplitOrEmpty('.');
            Authors = (file.Authors ?? throw new InvalidOperationException()).ToFixedList();
            Template = file.Template;
            References = references.ToFixedList();
        }
    }
}
