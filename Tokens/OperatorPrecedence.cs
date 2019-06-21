namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public enum OperatorPrecedence
    {
        Min = Assignment, // The minimum precedence
        AboveAssignment = Coalesce,
        Assignment = 1, // `=` `+=` `-=`
        Coalesce, // `??`
        LogicalOr, // `or`
        LogicalAnd, // `and`
        Equality, // `==` `≠`
        Relational, // `<` `<=` `>` `>=` `<:`
        Range, // `..` `..<`
        Additive, // `+` `-`
        Multiplicative, // `*` `/`
        Lifetime, // `$`
        Unary, // `+` `-` `not` `?`
        Primary // `f()` `.` `[]`
    }
}
