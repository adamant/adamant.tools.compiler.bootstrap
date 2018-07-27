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
        public CodeReference CodeReference { get; }
        public CodeText Code { get; }
        public TSyntax Root { get; }

        public SyntaxTree(CodeReference codeReference, CodeText code, TSyntax root)
        {
            CodeReference = codeReference;
            Code = code;
            Root = root;
        }
    }
}
