using Newtonsoft.Json;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Config
{
    public class ProjectDependencyConfig
    {
        [JsonProperty("path")]
        public string? Path { get; set; }

        [JsonProperty("trusted")]
        public bool? Trusted { get; set; }
    }
}
