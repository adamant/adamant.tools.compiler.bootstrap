namespace Adamant.Tools.Compiler.Bootstrap.Core.Syntax
{
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
    public enum TokenKind
    {
        Unexpected = -1,
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
        DotDot, // `..` U+002E, U+002E
        Colon, // `:` U+003A
        Question, // `?` U+003F
        Pipe, // `|` U+007C
        Dollar, // `$` U+0024
        RightArrow, // `→` U+2192, `->` U+002D, U+003E
        AtSign, // `@` U+0040 (named at sign to distinguish it from the word "at")
        Caret, // `^` U+005E
        Plus, // `+` U+002B
        Minus, // `-` U+002D
        Asterisk, // `*` U+002A
        Slash, // `/` U+002F
        Equals, // `=` U+003D
        EqualsEquals, // `==` U+003D, U+003D
        NotEqual, // `≠` U+2260, `=/=` U+003D, U+002F, U+003D
        GreaterThan, // `>` U+003E
        GreaterThanOrEqual, // `≥` U+2265, `⩾` U+2A7E, `>=` U+003E, U+003D
        LessThan, // `<` U+003C
        LessThanOrEqual, // `≤` U+2264, `⩽` U+2A7D, `<=` U+003C, U+003D
        PlusEquals, // `+=` U+002B, U+003D
        MinusEquals, // `-=` U+002D, U+003D
        AsteriskEquals, // `*=` U+002A, U+003D
        SlashEquals, // `/=` U+002F, U+003D

        // Identifiers
        Identifier,

        // Literals
        IntegerLiteral,
        StringLiteral,

        // Keywords
        PublicKeyword,
        ProtectedKeyword,
        PrivateKeyword,
        LetKeyword,
        VarKeyword,
        VoidKeyword,
        IntKeyword,
        UIntKeyword,
        ByteKeyword,
        SizeKeyword,
        BoolKeyword,
        StringKeyword,
        ReturnKeyword,
        ClassKeyword,
        FunctionKeyword,
        NewKeyword,
        InitKeyword,
        DeleteKeyword,
        OwnedKeyword,
        NamespaceKeyword,
        UsingKeyword,
        ForeachKeyword,
        InKeyword,
        IfKeyword,
        ElseKeyword,
        AndKeyword,
        OrKeyword,
        XorKeyword,
        StructKeyword,
        EnumKeyword,
        UnsafeKeyword,
        SafeKeyword,
        SelfKeyword,
        SelfTypeKeyword,
        BaseKeyword,
    }
}
