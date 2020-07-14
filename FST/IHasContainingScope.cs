using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IHasContainingScope
    {
        [DisallowNull] LexicalScope? ContainingScope { get; set; }
    }
}
