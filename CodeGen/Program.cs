using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public static class Program
    {
        public const string DirectiveMarker = "◊";

        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "Main method catching all exceptions to prevent exit")]
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Expected exactly one argument: CompilerCodeGen <grammar file>");
                return -1;
            }

            try
            {
                Console.WriteLine("~~~~~~ Compiler Code Generator");
                var inputPath = args[0];
                var outputPath = Path.ChangeExtension(inputPath, ".tree.cs");
                Console.WriteLine($"Input:  {inputPath}");
                Console.WriteLine($"Output: {outputPath}");

                var inputFile = File.ReadAllText(inputPath)
                                ?? throw new InvalidOperationException("null from reading input file");
                var grammar = Parser.ReadGrammarConfig(inputFile);
                var code = CodeBuilder.Generate(grammar);
                // Only write if changed so VS doesn't think we constantly change the file and need to recompile
                var previousCode = File.Exists(outputPath) ? File.ReadAllText(outputPath, new UTF8Encoding(false, true)) : null;
                if (code != previousCode)
                    File.WriteAllText(outputPath, code);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return -1;
            }
        }
    }
}
