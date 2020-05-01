using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers
{
    /// <summary>
    /// Fields of `self` are treated similar to variables. Effectively, the self
    /// object owns them.
    /// </summary>
    internal class FieldPlaceIdentifier : PlaceIdentifier
    {
        public SimpleName FieldName { get; }

        public FieldPlaceIdentifier(SimpleName fieldName)
        {
            FieldName = fieldName;
        }
    }
}
