namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface ITypeKindKeywordTokenPlace : IKeywordTokenPlace { }

    public partial interface IClassKeywordTokenPlace : ITypeKindKeywordTokenPlace { }
    public partial interface IStructKeywordTokenPlace : ITypeKindKeywordTokenPlace { }
}
