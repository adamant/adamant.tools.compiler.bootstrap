using System.Threading.Tasks;
using Adamant.Tools.Compiler.Bootstrap.Forge.Build;
using Adamant.Tools.Compiler.Bootstrap.Forge.Config;
using McMaster.Extensions.CommandLineUtils;

namespace Adamant.Tools.Compiler.Bootstrap.Forge
{
    public static class Program
    {
#if DEBUG
        public const bool DefaultAllowParallel = false;
#else
        public const bool DefaultAllowParallel = true;
#endif

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Name = "forge",
                Description = "Adamant's package manager and build tool"
            };

            app.HelpOption();

            var allowParallelOption = app.Option<bool>("--allow-parallel", "Allow parallel processing", CommandOptionType.SingleOrNoValue, true);
            var maxConcurrencyOption = app.Option<int>("--max-concurrency",
                "The max number of tasks to run concurrently",
                CommandOptionType.SingleValue, true);


            app.Command("build", cmd =>
                {
                    cmd.Description = "Compile a package and all of its dependencies";
                    var packageOption = cmd.Option("-p|--package <Package-Path>",
                        "Package to build", CommandOptionType.SingleValue);
                    var verboseOption = cmd.Option("-v|--verbose", "Use verbose output",
                        CommandOptionType.NoValue);
                    cmd.OnExecute(async () =>
                    {
                        var allowParallel = allowParallelOption.OptionalValue() ?? DefaultAllowParallel;
                        var maxConcurrency = maxConcurrencyOption.OptionalValue();
                        var verbose = verboseOption.HasValue();
                        var packagePath = packageOption.Value() ?? ".";

                        await Build(packagePath, verbose, allowParallel, maxConcurrency).ConfigureAwait(false);
                    });
                });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }

        private static async Task Build(
            string packagePath,
            bool verbose,
            bool allowParallel,
            int? maxConcurrency)
        {
            var configs = new ProjectConfigSet();
            configs.Load(packagePath);
            var projectSet = new ProjectSet();
            projectSet.AddAll(configs);
            var taskScheduler = NewTaskScheduler(
                allowParallel,
                maxConcurrency);
            await projectSet.Build(taskScheduler, verbose).ConfigureAwait(false);
        }

        private static TaskScheduler NewTaskScheduler(bool allowParallel, int? maxConcurrency)
        {
            ConcurrentExclusiveSchedulerPair taskSchedulerPair;
            if (maxConcurrency is int concurrency)
                taskSchedulerPair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, concurrency);
            else
                taskSchedulerPair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default);

            return allowParallel
                ? taskSchedulerPair.ConcurrentScheduler
                : taskSchedulerPair.ExclusiveScheduler;
        }
    }
}
