using System;
using System.Collections.Generic;
using System.IO;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Config
{
    internal class ProjectConfig
    {
        public const string FileName = "forge-project.vson";

        [JsonIgnore]
        public string FullPath { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("authors")]
        public List<string> Authors { get; set; }

        [JsonProperty("template")]
        public ProjectTemplate Template { get; set; }

        [JsonProperty("dependencies")]
        public Dictionary<string, ProjectDependencyConfig> Dependencies { get; set; } = new Dictionary<string, ProjectDependencyConfig>();

        [NotNull]
        public static ProjectConfig Load(string path)
        {
            var extension = Path.GetExtension(path);
            string projectFilePath;
            if (extension == "vson")
            {
                projectFilePath = path;
            }
            else if (string.IsNullOrEmpty(extension))
            {
                // Assume it is a directory
                projectFilePath = Path.Combine(path, FileName);
            }
            else
            {
                throw new Exception($"Unexpected project file extension '.{extension}'");
            }

            projectFilePath = Path.GetFullPath(projectFilePath);

            using (var file = new JsonTextReader(File.OpenText(projectFilePath)))
            {
                var serializer = new JsonSerializer();
                var projectFile = serializer.Deserialize<ProjectConfig>(file).AssertNotNull();
                projectFile.FullPath = projectFilePath;
                return projectFile;
            }
        }
    }
}
