using System.Collections;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Config
{
    /// <summary>
    /// A set of project configs. Note that this assumes the name used to refer to a project
    /// matches the name in the project.
    /// </summary>
    internal class ProjectConfigSet : IEnumerable<ProjectConfig>
    {
        private readonly Dictionary<string, ProjectConfig> configs = new Dictionary<string, ProjectConfig>();

        public ProjectConfig Load(string packagePath)
        {
            var config = ProjectConfig.Load(packagePath);
            if (configs.TryGetValue(config.FullPath, out var existingConfig))
                return existingConfig;

            configs.Add(config.Name, config);
            foreach (var dependency in config.Dependencies)
                Load(dependency.Value.Path);

            return config;
        }

        public ProjectConfig this[string name] => configs[name];

        public IEnumerator<ProjectConfig> GetEnumerator()
        {
            return configs.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
