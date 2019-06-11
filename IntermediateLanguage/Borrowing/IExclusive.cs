namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim with exclusive access either owns or borrows the lifetime
    /// </summary>
    public interface IExclusive
    {
        IClaimHolder Holder { get; }
        Lifetime Lifetime { get; }
    }
}
