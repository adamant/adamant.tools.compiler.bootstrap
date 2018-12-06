using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Config
{
    internal class ProjectConfigSet : IEnumerable<ProjectConfig>
    {
        private readonly Dictionary<string, ProjectConfig> configs = new Dictionary<string, ProjectConfig>();

        public ProjectConfig Load(string packagePath)
        {
            var config = ProjectConfig.Load(packagePath);
            if (configs.TryGetValue(config.FullPath, out var existingConfig))
                return existingConfig;

            configs.Add(config.FullPath, config);
            foreach (var dependency in config.Dependencies)
                Load(dependency.Value.Path);

            return config;
        }

        public ProjectConfig AtPath(string path)
        {
            if (!Path.HasExtension(path))
                path = Path.Combine(path, ProjectConfig.FileName);
            var fullPath = Path.GetFullPath(path);
            return configs[fullPath];
        }

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
