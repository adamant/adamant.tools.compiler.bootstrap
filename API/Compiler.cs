using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace API
{
    public class Compiler
    {
        public Package CompilePackage(IEnumerable<CodeFile> files)
        {
            var syntaxTrees = Parse(files);
            var packageSyntax = new PackageSyntax(syntaxTrees);

            var analyzer = new SemanticAnalyzer();
            analyzer.Analyze(packageSyntax);

            throw new NotImplementedException();
        }

        private static IEnumerable<SyntaxTree> Parse(IEnumerable<CodeFile> files)
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
