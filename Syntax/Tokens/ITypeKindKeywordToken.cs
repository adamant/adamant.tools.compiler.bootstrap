namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface ITypeKindKeywordToken : IKeywordToken { }

    public partial interface IClassKeywordToken : ITypeKindKeywordToken { }
    public partial interface IStructKeywordToken : ITypeKindKeywordToken { }
}
