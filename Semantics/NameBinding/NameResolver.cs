using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.NameBinding
{
    /// <summary>
    /// Used during type checking to resolve names that may be overloaded
    /// </summary>
    public class NameResolver : ExpressionVisitor<Void>
    {
    }
}
