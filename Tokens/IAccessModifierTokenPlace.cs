namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IAccessModifierTokenPlace : IKeywordTokenPlace { }

    public partial interface IPublicKeywordTokenPlace : IAccessModifierTokenPlace { }
    public partial interface IProtectedKeywordTokenPlace : IAccessModifierTokenPlace { }
    public partial interface IPrivateKeywordTokenPlace : IAccessModifierTokenPlace { }
    public partial interface IInternalKeywordTokenPlace : IAccessModifierTokenPlace { }
}
