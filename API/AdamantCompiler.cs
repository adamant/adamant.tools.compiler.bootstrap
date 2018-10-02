using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class AdamantCompiler
    {
        public Package CompilePackage(IEnumerable<CodeFile> files)
        {
            var compilationUnits = Parse(files);
            var packageSyntax = new PackageSyntax(compilationUnits);

            var analyzer = new SemanticAnalyzer();
            var analysis = analyzer.Analyze(packageSyntax);

            return analysis.Package;
        }

        private static IList<CompilationUnitSyntax> Parse(IEnumerable<CodeFile> files)
        {
            var lexer = new Lexer();
            var parser = new Parser();

            return files
#if RELEASE
                .AsParallel()
#endif
                .Select(file =>
                {
                    var tokens = lexer.Lex(file);
                    return parser.Parse(file, tokens);
                })
                .ToList();
        }
    }
}
