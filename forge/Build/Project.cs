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
        [NotNull] public string Name { get; }
        [NotNull] public IReadOnlyList<string> Authors { get; }
        public ProjectTemplate Template { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ProjectReference> References { get; }

        public Project(
            [NotNull] ProjectConfig file,
            [NotNull][ItemNotNull] IEnumerable<ProjectReference> references)
        {
            Requires.NotNull(nameof(file), file);
            Path = System.IO.Path.GetDirectoryName(file.FullPath).AssertNotNull();
            Name = file.Name ?? throw new InvalidOperationException();
            Authors = (file.Authors ?? throw new InvalidOperationException()).ToReadOnlyList();
            Template = file.Template;
            References = references.ToReadOnlyList();
        }
    }
}
