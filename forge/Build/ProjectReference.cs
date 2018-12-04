using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Build
{
    internal class ProjectReference
    {
        [NotNull] public string Name { get; }
        [NotNull] public Project Project { get; }
        public bool Trusted { get; }

        public ProjectReference([NotNull] string name, [NotNull]  Project project, bool trusted)
        {
            Name = name;
            Project = project;
            Trusted = trusted;
        }
    }
}
