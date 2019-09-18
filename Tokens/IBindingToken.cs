using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(ILetKeywordToken),
        typeof(IVarKeywordToken))]
    public interface IBindingToken : IKeywordToken { }

    public partial interface ILetKeywordToken : IBindingToken { }
    public partial interface IVarKeywordToken : IBindingToken { }
}
