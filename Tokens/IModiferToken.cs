using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IAccessModifierToken),
        //typeof(IAbstractKeywordToken),
        //typeof(IImplicitKeywordToken),
        //typeof(IExplicitKeywordToken),
        typeof(IMutableKeywordToken),
        //typeof(IOverrideKeywordToken),
        typeof(IMoveKeywordToken),
        //typeof(IRefKeywordToken),
        typeof(ISafeKeywordToken),
        typeof(IUnsafeKeywordToken)
        )]
    public partial interface IModiferToken : IKeywordToken { }

    public partial interface IAccessModifierToken : IModiferToken { }
    //public partial interface IAbstractKeywordToken : IModiferToken { }
    //public partial interface IImplicitKeywordToken : IModiferToken { }
    //public partial interface IExplicitKeywordToken : IModiferToken { }
    public partial interface IMutableKeywordToken : IModiferToken { }
    //public partial interface IOverrideKeywordToken : IModiferToken { }
    public partial interface IMoveKeywordToken : IModiferToken { }
    //public partial interface IRefKeywordToken : IModiferToken { }
    public partial interface ISafeKeywordToken : IModiferToken { }
    public partial interface IUnsafeKeywordToken : IModiferToken { }
}
