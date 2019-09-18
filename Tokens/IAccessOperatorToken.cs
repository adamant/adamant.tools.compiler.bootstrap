using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IDotToken)
        //typeof(IQuestionDotToken),
        //typeof(ICaretDotToken)
        )]
    public partial interface IAccessOperatorToken : IOperatorToken { }

    public partial interface IDotToken : IAccessOperatorToken { }
    //public partial interface IQuestionDotToken : IAccessOperatorToken { }
    //public partial interface ICaretDotToken : IAccessOperatorToken { }
}
