using System;
using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public class Program
    {
        public const string DirectiveMarker = "◊";

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
                var outputPath = Path.ChangeExtension(inputPath, "cs");
                Console.WriteLine($"Input:  {inputPath}");
                Console.WriteLine($"Output: {outputPath}");

                var grammar = Parser.ReadGrammarConfig(inputPath);
                var code = CodeBuilder.Generate(grammar);
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
