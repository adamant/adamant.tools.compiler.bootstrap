namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim that a variable shares the lifetime
    /// </summary>
    public interface IShares
    {
        Lifetime Lifetime { get; }
    }
}
