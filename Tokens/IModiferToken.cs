namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IModiferToken : IKeywordToken { }

    public partial interface IAccessModifierToken : IModiferToken { }
    public partial interface IAbstractKeywordToken : IModiferToken { }
    public partial interface IImplicitKeywordToken : IModiferToken { }
    public partial interface IExplicitKeywordToken : IModiferToken { }
    public partial interface IMutableKeywordToken : IModiferToken { }
    public partial interface IOverrideKeywordToken : IModiferToken { }
    public partial interface IMoveKeywordToken : IModiferToken { }
    public partial interface IRefKeywordToken : IModiferToken { }
    public partial interface ISafeKeywordToken : IModiferToken { }
    public partial interface IUnsafeKeywordToken : IModiferToken { }
}
