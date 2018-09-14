using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Config
{
    internal class ProjectFile
    {
        [JsonIgnore]
        public string FullPath { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("authors")]
        public List<string> Authors { get; set; }

        [JsonProperty("template")]
        public ProjectTemplate Template { get; set; }

        public static ProjectFile Load(string path)
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
                projectFilePath = Path.Combine(path, "forge-project.vson");
            }
            else
            {
                throw new Exception($"Unexpected project file extension '.{extension}'");
            }

            projectFilePath = Path.GetFullPath(projectFilePath);

            using (var file = new JsonTextReader(File.OpenText(projectFilePath)))
            {
                var serializer = new JsonSerializer();
                var projectFile = serializer.Deserialize<ProjectFile>(file);
                projectFile.FullPath = projectFilePath;
                return projectFile;
            }
        }
    }
}
