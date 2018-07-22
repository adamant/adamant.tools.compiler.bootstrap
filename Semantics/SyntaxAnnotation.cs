using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SyntaxAnnotation<TValue> : Dictionary<SyntaxBranchNode, TValue>
    {
    }
}
