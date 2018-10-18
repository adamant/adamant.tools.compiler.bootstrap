namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IPrimitiveTypeToken : IToken
    {
    }

    public partial interface IVoidKeywordToken : IPrimitiveTypeToken { }
    public partial interface IIntKeywordToken : IPrimitiveTypeToken { }
    public partial interface IUIntKeywordToken : IPrimitiveTypeToken { }
    public partial interface IBoolKeywordToken : IPrimitiveTypeToken { }
    public partial interface IByteKeywordToken : IPrimitiveTypeToken { }
    public partial interface IStringKeywordToken : IPrimitiveTypeToken { }
    public partial interface INeverKeywordToken : IPrimitiveTypeToken { }
    public partial interface ISizeKeywordToken : IPrimitiveTypeToken { }
    public partial interface ITypeKeywordToken : IPrimitiveTypeToken { }
}
