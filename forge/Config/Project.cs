using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Config
{
    internal class Project
    {
        [NotNull] public string Path { get; }
        [NotNull] public string Name { get; }
        [NotNull] public IReadOnlyList<string> Authors { get; }
        [NotNull] public ProjectTemplate Template { get; }
        [NotNull] public IReadOnlyList<Project> ReferencedProjects { get; }

        public Project([NotNull] ProjectFile file)
        {
            Requires.NotNull(nameof(file), file);
            Path = System.IO.Path.GetDirectoryName(file.FullPath);
            Name = file.Name ?? throw new InvalidOperationException();
            Authors = (file.Authors ?? throw new InvalidOperationException()).ToList().AsReadOnly().AssertNotNull();
            Template = file.Template;
            ReferencedProjects = new List<Project>().AsReadOnly().AssertNotNull();
        }
    }
}
