namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IAccessOperatorToken : IOperatorToken { }

    public partial interface IDotToken : IAccessOperatorToken { }
    public partial interface IQuestionDotToken : IAccessOperatorToken { }
    public partial interface ICaretDotToken : IAccessOperatorToken { }
}
