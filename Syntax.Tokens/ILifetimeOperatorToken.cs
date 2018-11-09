namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface ILifetimeOperatorToken : IOperatorToken
    {
    }

    public partial interface IDollarToken : ILifetimeOperatorToken { }
    public partial interface IDollarLessThanToken : ILifetimeOperatorToken { }
    public partial interface IDollarLessThanNotEqualToken : ILifetimeOperatorToken { }
    public partial interface IDollarGreaterThanToken : ILifetimeOperatorToken { }
    public partial interface IDollarGreaterThanNotEqualToken : ILifetimeOperatorToken { }
}
