using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface ICapabilityTypeSyntax : ITypeSyntax
    {
        ITypeSyntax ReferentType { get; }
        ReferenceCapability Capability { get; }
    }
}
