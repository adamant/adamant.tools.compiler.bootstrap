namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    /// Roslyn very consistently names their tokens composed of Unicode Symbols
    /// as the concatination of the symbol names followed by "Token". However,
    /// it abbreviates the symbol names by removing filler words like "sign" and
    /// "mark". This has the benifit of consistency even for difficult cases
    /// like `=` and `==`. It is somewhat odd though for cases like `->` being
    /// named "MinusGreaterThanToken".
    ///
    /// It is tempting to name symbol tokens for their meaning, for example `=`
    /// would be "assign" while `==` would be "equal". However, this is problematic
    /// for symbols like `+` which could mean "add" or "positive" depending on
    /// whether it is a binary or unary operator. Also, if `->` is named arrow,
    /// then if we support `→` (U+2192) then there is not a good name for it
    /// distinct from the other. On the other hand, if `->` and `→` are meant to
    /// be synonynms then perhaps it makes sense to use the single symbol name?
    ///
    /// I really like the idea of supporting Unicode correct operators like not
    /// equal. I'm going to name each symbol token after the desired Unicode
    /// code point, dropping filler words.
    public enum TokenKind
    {
        EndOfFile = 0,
        LeftCurlyBracket, // `{` U+007B
        RightCurlyBracket, // `}` U+007D
        Plus, // `+` U+002B
        Equals, // `=` U+003D
        EqualsEquals, // `==` U+003D, U+003D
        PlusEquals, // `+=` U+002B, U+003D
        NotEqual, // `≠` U+2260 or `=/=` U+003D, , U+003D
        Solidus, // `/` U+002F
        SolidusEquals, // `/=` U+002F, U+003D
        Identifier,
        EscapedIdentifier, // TODO this should just be a kind of indentifier
        EscapedStringIdentifier, // TODO this should just be a kind of indentifier
    }
}
