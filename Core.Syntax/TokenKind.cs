namespace Core.Syntax
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
    /// equal. However, some of the Unicode character names are so weird that
    /// it doesn't really make sense to me to use them.  I'm going to name
    /// things after their single character version, but not follow Unicode
    /// naming when it doesn't make sense
    public enum TokenKind
    {
        EndOfFile = 0,

        // Symbols
        OpenBrace, // `{` U+007B
        CloseBrace, // `}` U+007D
        OpenParen, // `(` U+0028
        CloseParen, // `)` U+0029
        OpenBracket, // `[` U+005B
        CloseBracket, // `]` U+005D
        Semicolon, // `;` U+003B
        Comma, // `,` U+002C
        Dot, // `.` U+002E
        Colon, // `:` U+003A
        Question, // `?` U+003F
        Pipe, // `|` U+007C
        RightArrow, // `→` U+2192
        AtSign, // `@` U+0040 (named at sign to distinguish it from the word "at")
        Caret, // `^` U+005E
        Plus, // `+` U+002B
        Minus, // `-` U+002D
        Asterisk, // `*` U+002A
        Slash, // `/` U+002F
        Equals, // `=` U+003D
        EqualsEquals, // `==` U+003D, U+003D
        NotEqual, // `≠` U+2260 or `=/=` U+003D, , U+003D
        GreaterThan, // `>` U+003E
        GreaterThanOrEqual, // `≥` U+2265
        LessThan, // `<` U+003C
        LessThanOrEqual, // `≤` U+2264
        PlusEquals, // `+=` U+002B, U+003D
        MinusEquals, // `-=` U+002D, U+003D
        AsteriskEquals, // `*=` U+002A, U+003D
        SlashEquals, // `/=` U+002F, U+003D

        // Identifiers
        Identifier,

        // Literals
        StringLiteral,

        // Keywords
        PublicKeyword,
        LetKeyword,
        VarKeyword,
        VoidKeyword,
        IntKeyword,
        BoolKeyword,
        ReturnKeyword,
        ClassKeyword,
        NewKeyword,
        DeleteKeyword,
    }
}
