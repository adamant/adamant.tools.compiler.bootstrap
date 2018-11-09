using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// Ideally tokens would be Adamant enum structs. That would maximize
    /// performance and minimize memory footprint. Since C# doesn't have an
    /// equivalent, a class is used. This allows for switching on type.
    ///
    /// Also ideally, missing tokens would be represented by a special token
    /// type that allowed for position. However, the C# type system doesn't make
    /// that work out well. So missing tokens are just null.
    ///
    /// Roslyn very consistently names their tokens composed of Unicode Symbols
    /// as the concatenation of the symbol names followed by "Token". However,
    /// it abbreviates the symbol names by removing filler words like "sign" and
    /// "mark". This has the benefit of consistency even for difficult cases
    /// like `=` and `==`. It is somewhat odd though for cases like `->` being
    /// named "MinusGreaterThanToken".
    ///
    /// It is tempting to name symbol tokens for their meaning, for example `=`
    /// would be "assign" while `==` would be "equal". However, this is problematic
    /// for symbols like `+` which could mean "add" or "positive" depending on
    /// whether it is a binary or unary operator. Also, if `->` is named arrow,
    /// then if we support `→` (U+2192) then there is not a good name for it
    /// distinct from the other. On the other hand, if `->` and `→` are meant to
    /// be synonyms then perhaps it makes sense to use the single symbol name?
    ///
    /// I really like the idea of supporting Unicode correct operators like not
    /// equal. However, some of the Unicode character names are so weird that
    /// it doesn't really make sense to me to use them.  I'm going to name
    /// things after their single character version, but not follow Unicode
    /// naming when it doesn't make sense
    public abstract partial class Token : ISyntaxNodeOrToken, IToken
    {
        public TextSpan Span { get; }

        protected Token(TextSpan span)
        {
            Span = span;
        }

        [Pure]
        [NotNull]
        public string Text([NotNull] CodeText code)
        {
            return Span.GetText(code.Text);
        }
    }
}
