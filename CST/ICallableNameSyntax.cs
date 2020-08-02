using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// The name of a function, method, constructor that is being invoked
    /// </summary>
    public partial interface ICallableNameSyntax : IHasContainingScope
    {
        [DisallowNull] IFunctionMetadata? ReferencedFunctionMetadata { get; set; }
    }
}
