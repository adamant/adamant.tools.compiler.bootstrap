namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public enum ReferenceCapability
    {
        Owned,
        OwnedMutable,
        Isolated,
        IsolatedMutable,
        Held,
        HeldMutable,
        Shared,
        Borrowed,
        Identity,
    }
}
