namespace Adamant.Tools.Compiler.Bootstrap.Forge.Build
{
    internal class ProjectReference
    {
        public string Name { get; }
        public Project Project { get; }
        public bool Trusted { get; }

        public ProjectReference(string name, Project project, bool trusted)
        {
            Name = name;
            Project = project;
            Trusted = trusted;
        }
    }
}
