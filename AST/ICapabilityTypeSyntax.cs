using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ICapabilityTypeSyntax : ITypeSyntax
    {
        ITypeSyntax ReferentType { get; }
        ReferenceCapability Capability { get; }
    }
}
