namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IBindingToken : IKeywordToken { }

    public partial interface ILetKeywordToken : IBindingToken { }
    public partial interface IVarKeywordToken : IBindingToken { }
}
