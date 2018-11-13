using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Forge.Config;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Build
{
    internal class Project
    {
        [NotNull] public string Path { get; }
        [NotNull] public FixedList<string> RootNamespace { get; }
        [NotNull] public string Name { get; }
        [NotNull] public FixedList<string> Authors { get; }
        public ProjectTemplate Template { get; }
        [NotNull] [ItemNotNull] public FixedList<ProjectReference> References { get; }

        public Project(
            [NotNull] ProjectConfig file,
            [NotNull][ItemNotNull] IEnumerable<ProjectReference> references)
        {
            Requires.NotNull(nameof(file), file);
            Path = System.IO.Path.GetDirectoryName(file.FullPath).NotNull();
            Name = file.Name ?? throw new InvalidOperationException();
            RootNamespace = (file.RootNamespace ?? "").SplitOrEmpty('.');
            Authors = (file.Authors ?? throw new InvalidOperationException()).ToFixedList();
            Template = file.Template;
            References = references.ToFixedList();
        }
    }
}
