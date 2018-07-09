using McMaster.Extensions.CommandLineUtils;
using System;

namespace Adamant.Tools.Compiler.Bootstrap.AdamantC
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();

            var inputFilesOption = app.Argument("<Input-Files>", "Input Code Files", true).IsRequired();
            var outputOption = app.Option("-o|--output <Output-File>", "Output Code File", CommandOptionType.SingleValue).IsRequired();
            var verboseOption = app.Option("-v|--verbose", "Verbose Logging", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                var inputFiles = string.Join(';', inputFilesOption.Values);
                var outputFile = outputOption.Value();
                var verbose = verboseOption.HasValue(); // Strange since we didn't pass a value
                Console.WriteLine("Input Files=" + inputFiles);
                Console.WriteLine("Output File=" + outputFile);
                Console.WriteLine("Verbose=" + verbose);
            });

            return app.Execute(args);
        }
    }
}
