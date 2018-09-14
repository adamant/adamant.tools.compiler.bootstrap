using Adamant.Tools.Compiler.Bootstrap.Forge.Build;
using Adamant.Tools.Compiler.Bootstrap.Forge.Config;
using McMaster.Extensions.CommandLineUtils;

namespace Adamant.Tools.Compiler.Bootstrap.Forge
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Name = "forge",
                Description = "Adamant's package manager and build tool"
            };

            app.HelpOption();

            app.Command("build", cmd =>
                {
                    cmd.Description = "Compile a package and all of its dependencies";
                    var packageOption = cmd.Option("-p|--package <Package-Path>",
                        "Package to build", CommandOptionType.SingleValue);
                    var verboseOption = cmd.Option("-v|--verbose", "Use verbose output",
                        CommandOptionType.NoValue);
                    cmd.OnExecute(() =>
                    {
                        var verbose = verboseOption.HasValue();
                        var packagePath = packageOption.Value() ?? ".";
                        var projectFile = ProjectFile.Load(packagePath);
                        var buildSequence = new BuildSequence() { new Project(projectFile) };
                        buildSequence.Build(verbose);
                    });
                });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }
    }
}
