using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    internal class ModifierParser : RecursiveDescentParser
    {
        public ModifierParser(ITokenIterator<IEssentialToken> tokens)
            : base(tokens) { }

        public IAccessModifierToken? ParseAccessModifier()
        {
            return Tokens.Current switch
            {
                IAccessModifierToken _ => Tokens.RequiredToken<IAccessModifierToken>(),
                _ => null
            };
        }

        public IMutableKeywordToken? ParseMutableModifier()
        {
            return Tokens.Current is IMutableKeywordToken ? Tokens.RequiredToken<IMutableKeywordToken>() : null;
        }

        public void ParseEndOfModifiers()
        {
            while (!(Tokens.Current is IEndOfFileToken))
            {
                Tokens.UnexpectedToken();
            }
        }
    }
}
