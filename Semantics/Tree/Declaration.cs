using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Tree
{
    internal abstract class Declaration : AbstractSyntax, IDeclaration
    {
        public Symbol Symbol => throw new System.NotImplementedException();
    }
}
