namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        /// <summary>
        /// This gathers up the modifiers and removes duplicate types. For example
        /// there should be only one access modifer and only one of safe/unsafe.
        /// </summary>
        //[NotNull]
        //public FixedList<IModiferToken> ParseModifiers()
        //{
        //    var modifiers = new List<IModiferToken>();
        //    IModiferToken modifer;
        //    while ((modifer = Accept<IModiferToken>()) != null)
        //        modifiers.Add(modifer);

        //    // TODO remove "duplicates"

        //    return modifiers.ToFixedList();
        //}
    }
}
