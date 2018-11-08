using System.Diagnostics;
using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CLangCompiler
    {
        public int Compile(
            [NotNull] ICompilerOutput output,
            [NotNull, ItemNotNull] string[] sourceFiles,
            [NotNull, ItemNotNull] string[] headerSearchPaths,
            [NotNull] string outputPath)
        {
            Requires.NotNull(nameof(output), output);
            Requires.NotNull(nameof(sourceFiles), sourceFiles);
            Requires.NotNull(nameof(headerSearchPaths), headerSearchPaths);
            Requires.NotNull(nameof(outputPath), outputPath);

            // used to have: -Wno-incompatible-pointer-types
            var options = "-std=c11 -fsanitize=undefined -fsanitize=integer -fsanitize=nullability -Wall  -Wno-unused-label";
            // Next thing is needed for windows
            options += " -Xclang -flto-visibility-public-std";
            if (Path.GetExtension(outputPath) == ".dll") // TODO take this as an argument or something
                options += " --shared";
            var sources = string.Join(' ', sourceFiles);
            var headers = string.Join(' ', headerSearchPaths.Select(h => "--include-directory " + h));
            var arguments = $"{sources} -o {outputPath} {headers} {options}";
            output.WriteLine("clang arguments:");
            output.WriteLine(arguments);
            var startInfo = new ProcessStartInfo("clang", arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            var process = new Process() { StartInfo = startInfo };
            // To prevent blocking on a full buffer, we have to process output as it happens
            process.OutputDataReceived += ProcessOutput;
            process.ErrorDataReceived += ProcessOutput;
            output.WriteLine("clang output:");
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            return process.ExitCode;

            void ProcessOutput(object s, DataReceivedEventArgs e)
            {
                output.WriteLine(e.Data);
            }
        }
    }
}
