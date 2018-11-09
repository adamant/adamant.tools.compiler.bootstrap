namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IBindingKeywordToken : IKeywordToken
    {
    }


    public partial interface ILetKeywordToken : IBindingKeywordToken { }
    public partial interface IVarKeywordToken : IBindingKeywordToken { }
}
