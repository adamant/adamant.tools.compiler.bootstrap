using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    /// <summary>
    /// Metadata for a function
    /// </summary>
    public interface IFunctionMetadata : IParentMetadata
    {
        IEnumerable<IBindingMetadata> Parameters { get; }
        DataType ReturnType { get; }
    }
}
