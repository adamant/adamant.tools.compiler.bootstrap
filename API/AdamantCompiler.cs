using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class AdamantCompiler
    {
        /// <summary>
        /// Whether to store the liveness analysis for each function and method.
        /// Default Value: false
        /// </summary>
        public bool SaveLivenessAnalysis { get; set; } = false;

        /// <summary>
        /// Whether to store the borrow checker claims for each function and method.
        /// Default Value: false
        /// </summary>
        public bool SaveReachabilityGraphs { get; set; } = false;

        public Task<Package> CompilePackageAsync(
            string name,
            IEnumerable<ICodeFileSource> files,
            FixedDictionary<string, Task<Package>> referenceTasks)
        {
            return CompilePackageAsync(name, files, referenceTasks, TaskScheduler.Default);
        }

        public async Task<Package> CompilePackageAsync(
            string name,
            IEnumerable<ICodeFileSource> fileSources,
            FixedDictionary<string, Task<Package>> referenceTasks,
            TaskScheduler taskScheduler)
        {
            var lexer = new Lexer();
            var parser = new CompilationUnitParser();
            var parseBlock = new TransformBlock<ICodeFileSource, ICompilationUnitSyntax>(
                async (fileSource) =>
                {
                    var file = await fileSource.LoadAsync().ConfigureAwait(false);
                    var context = new ParseContext(file, new Diagnostics());
                    var tokens = lexer.Lex(context).WhereNotTrivia();
                    return parser.Parse(tokens);
                }, new ExecutionDataflowBlockOptions()
                {
                    TaskScheduler = taskScheduler,
                    EnsureOrdered = false,
                });

            foreach (var fileSource in fileSources)
                parseBlock.Post(fileSource);

            parseBlock.Complete();

            await parseBlock.Completion.ConfigureAwait(false);

            if (!parseBlock.TryReceiveAll(out var compilationUnits))
                throw new Exception("Not all compilation units are ready");

            var packageSyntax = new PackageSyntax(name, compilationUnits.ToFixedList());

            var analyzer = new SemanticAnalyzer()
            {
                SaveLivenessAnalysis = SaveLivenessAnalysis,
                SaveReachabilityGraphs = SaveReachabilityGraphs,
            };

            var referencePairs = await Task.WhenAll(referenceTasks
                                    .Select(async kv => (alias: kv.Key, package: await kv.Value.ConfigureAwait(false))))
                                    .ConfigureAwait(false);
            var references = referencePairs.ToFixedDictionary(r => r.alias, r => r.package);
            return analyzer.Check(packageSyntax, references);
        }

        public Package CompilePackage(
            string name,
            IEnumerable<ICodeFileSource> fileSources,
            FixedDictionary<string, Package> references)
        {
            return CompilePackage(name, fileSources.Select(s => s.Load()), references);
        }

        public Package CompilePackage(
            string name,
            IEnumerable<CodeFile> files,
            FixedDictionary<string, Package> references)
        {
            var lexer = new Lexer();
            var parser = new CompilationUnitParser();
            var compilationUnits = files
                .Select(file =>
                {
                    var context = new ParseContext(file, new Diagnostics());
                    var tokens = lexer.Lex(context).WhereNotTrivia();
                    return parser.Parse(tokens);
                })
                .ToFixedList();
            var packageSyntax = new PackageSyntax(name, compilationUnits);

            var analyzer = new SemanticAnalyzer()
            {
                SaveLivenessAnalysis = SaveLivenessAnalysis,
                SaveReachabilityGraphs = SaveReachabilityGraphs,
            };

            return analyzer.Check(packageSyntax, references);
        }
    }
}
