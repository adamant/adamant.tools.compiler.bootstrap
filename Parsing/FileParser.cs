using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class FileParser
    {
        [MustUseReturnValue]
        public CompilationUnitSyntax Parse([NotNull] ITokenIterator tokens)
        {
            var parser = new Parser(tokens, GlobalNamespaceName.Instance);
            return parser.ParseCompilationUnit();
        }
    }
}
