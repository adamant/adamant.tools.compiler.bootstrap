using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface ICapabilityTypeSyntax : ITypeSyntax
    {
        ITypeSyntax ReferentType { get; }
        ReferenceCapability Capability { get; }
    }
}
