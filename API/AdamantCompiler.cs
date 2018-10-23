using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class AdamantCompiler
    {
        public Task<Package> CompilePackageAsync([NotNull] string name, [NotNull][ItemNotNull] IEnumerable<ICodeFileSource> files)
        {
            return CompilePackageAsync(name, files, TaskScheduler.Default);
        }

        public Task<Package> CompilePackageAsync([NotNull] string name, [NotNull][ItemNotNull] IEnumerable<ICodeFileSource> fileSources, [CanBeNull] TaskScheduler taskScheduler)
        {
            var lexer = new Lexer();
            var parser = new Parser();
            var parseBlock = new TransformBlock<ICodeFileSource, CompilationUnitSyntax>(
                async (fileSource) =>
                {
                    var file = await fileSource.LoadAsync();
                    var tokens = lexer.Lex(file);
                    return parser.Parse(file, tokens);
                }, new ExecutionDataflowBlockOptions()
                {
                    TaskScheduler = taskScheduler,
                    EnsureOrdered = false,
                });

            foreach (var fileSource in fileSources)
                parseBlock.Post(fileSource);

            parseBlock.Complete();

            throw new NotImplementedException();
        }

        [NotNull]
        public Package CompilePackage([NotNull] string name, [NotNull][ItemNotNull] IEnumerable<ICodeFileSource> fileSources)
        {
            return CompilePackage(name, fileSources.Select(s => s.Load()));
        }

        [NotNull]
        public Package CompilePackage([NotNull] string name, [NotNull][ItemNotNull] IEnumerable<CodeFile> files)
        {
            var lexer = new Lexer();
            var parser = new Parser();

            var compilationUnits = files
                .Select(file =>
                {
                    var tokens = lexer.Lex(file);
                    return parser.Parse(file, tokens);
                })
                .ToSyntaxList();
            var packageSyntax = new PackageSyntax(name, compilationUnits);

            var analyzer = new SemanticAnalyzer();

            return analyzer.Analyze(packageSyntax);
        }
    }
}
