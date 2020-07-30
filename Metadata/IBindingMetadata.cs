using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    /// <summary>
    /// Metadata for a variable or field. Both of which are bindings using `let` or `var`
    /// </summary>
    public interface IBindingMetadata : IMetadata
    {
        DataType Type { get; }
        bool IsMutableBinding { get; }
    }
}
