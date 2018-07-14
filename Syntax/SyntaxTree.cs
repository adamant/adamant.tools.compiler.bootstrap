using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tree;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SyntaxTree
    {
        public SourceReference SourceReference { get; }
        public SourceText Source { get; }
        public CompilationUnitSyntax CompilationUnit { get; }

        public SyntaxTree(SourceReference sourceReference, SourceText source, CompilationUnitSyntax compilationUnit)
        {
            SourceReference = sourceReference;
            Source = source;
            CompilationUnit = compilationUnit;
        }
    }
}
