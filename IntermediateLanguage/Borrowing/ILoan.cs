namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A loan is either a borrow or share
    /// </summary>
    public interface ILoan
    {
        Lifetime Lifetime { get; }
    }
}
