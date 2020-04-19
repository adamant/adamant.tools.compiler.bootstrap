using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IReferenceCapabilityTypeSyntax : ITypeSyntax
    {
        ITypeSyntax ReferentType { get; }
        ReferenceCapability ReferenceCapability { get; }
    }
}
