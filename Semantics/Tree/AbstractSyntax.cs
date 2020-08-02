using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Tree
{
    internal abstract class AbstractSyntax : IAbstractSyntax
    {
        public TextSpan TextSpan => throw new System.NotImplementedException();
    }
}
