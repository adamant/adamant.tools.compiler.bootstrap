using System.Collections.ObjectModel;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Config
{
    internal class Project
    {
        public string Path { get; }
        public string Name { get; }
        public ReadOnlyCollection<string> Authors { get; }
        public ProjectTemplate Template { get; }

        public Project(ProjectFile file)
        {
            Path = System.IO.Path.GetDirectoryName(file.FullPath);
            Name = file.Name;
            Authors = file.Authors.ToList().AsReadOnly();
            Template = file.Template;
        }
    }
}
