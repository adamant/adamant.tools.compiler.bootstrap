using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class ModifierParser : ParserBase, IModifierParser
    {
        [NotNull] protected CodeFile File;
        [NotNull] protected ITokenIterator Tokens;

        public ModifierParser([NotNull] ITokenIterator tokens)
            : base(tokens)
        {
        }

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
        protected void Add([NotNull] Diagnostic diagnostic)
        {
            Tokens.Context.Diagnostics.Add(diagnostic);
        }
    }
}
