namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public enum OperatorPrecedence
    {
        Min = Assignment, // The minimum precendence
        Assignment = 1, //= += -=
        LogicalOr, // or
        LogicalAnd, // and
        Equality, // == =/=
        Relational, // < <= > >=
        Additive, // + -
        Multiplicative, // * / %
        Unary, // + - not
        Primary // f() . []
    }
}
