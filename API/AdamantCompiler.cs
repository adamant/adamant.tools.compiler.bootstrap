using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class AdamantCompiler
    {
        public Package CompilePackage(IEnumerable<CodeFile> files)
        {
            var syntaxTrees = Parse(files);
            var packageSyntax = new PackageSyntax(syntaxTrees);

            var analyzer = new SemanticAnalyzer();
            var package = analyzer.Analyze(packageSyntax);

            return package;
        }

        private static IEnumerable<SyntaxTree<CompilationUnitSyntax>> Parse(IEnumerable<CodeFile> files)
        {
            var lexer = new Lexer();
            var parser = new Parser();

            return files.AsParallel()
                .Select(file =>
                {
                    var tokens = lexer.Lex(file.Code);
                    return parser.Parse(file.Reference, file.Code, tokens);
                });
        }
    }
}
