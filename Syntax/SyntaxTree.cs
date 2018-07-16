using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tree;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SyntaxTree
    {
        public CodeReference CodeReference { get; }
        public CodeText Code { get; }
        public CompilationUnitSyntax CompilationUnit { get; }

        public SyntaxTree(CodeReference CodeReference, CodeText code, CompilationUnitSyntax compilationUnit)
        {
            this.CodeReference = CodeReference;
            Code = code;
            CompilationUnit = compilationUnit;
        }
    }
}
