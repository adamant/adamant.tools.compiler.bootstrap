namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IAccessModifierToken : IKeywordToken { }

    public partial interface IPublishedKeywordToken : IAccessModifierToken { }
    public partial interface IPublicKeywordToken : IAccessModifierToken { }
    public partial interface IProtectedKeywordToken : IAccessModifierToken { }
}
