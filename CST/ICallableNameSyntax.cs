using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// The name of a function, method, constructor that is being invoked
    /// </summary>
    public interface ICallableNameSyntax : ISyntax, IHasContainingScope
    {
        Name Name { get; }
        [DisallowNull] IFunctionMetadata? ReferencedFunctionMetadata { get; set; }
    }
}
