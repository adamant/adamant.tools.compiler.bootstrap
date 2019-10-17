using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IHasContainingScope
    {
        [DisallowNull] LexicalScope? ContainingScope { get; set; }
    }
}
