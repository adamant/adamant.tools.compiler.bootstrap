namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public enum OperatorPrecedence
    {
        Min = Assignment, // The minimum precedence
        Assignment = 1, // `=` `+=` `-=`
        LogicalOr, // `or`
        LogicalAnd, // `and`
        Equality, // `==` `=/=`
        Relational, // `<` `<=` `>` `>=`
        Range, // `..` `..<`
        Additive, // `+` `-`
        Multiplicative, // `*` `/`
        Unary, // `+` `-` `not`
        Primary // f() . []
    }
}
