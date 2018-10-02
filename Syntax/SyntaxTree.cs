using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    // TODO consider getting rid of this and putting code stuff on CompilationUnit
    // Syntax tree is supposed to allow fragments of trees that are still associated
    // with files. That doesn't seem useful here
    public class SyntaxTree<TSyntax>
        where TSyntax : SyntaxNode
    {
        public CodeFile File { get; }
        public TSyntax Root { get; }

        public SyntaxTree(CodeFile file, TSyntax root)
        {
            File = file;
            Root = root;
        }
    }
}
