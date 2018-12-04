namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IPrimitiveTypeToken : IToken { }

    public partial interface IVoidKeywordToken : IPrimitiveTypeToken { }
    public partial interface INeverKeywordToken : IPrimitiveTypeToken { }
    public partial interface IBoolKeywordToken : IPrimitiveTypeToken { }
    public partial interface IAnyKeywordToken : IPrimitiveTypeToken { }
    public partial interface ITypeKeywordToken : IPrimitiveTypeToken { }

    public partial interface IInt8KeywordToken : IPrimitiveTypeToken { }
    public partial interface IByteKeywordToken : IPrimitiveTypeToken { }
    public partial interface IInt16KeywordToken : IPrimitiveTypeToken { }
    public partial interface IUInt16KeywordToken : IPrimitiveTypeToken { }
    public partial interface IIntKeywordToken : IPrimitiveTypeToken { }
    public partial interface IUIntKeywordToken : IPrimitiveTypeToken { }
    public partial interface IInt64KeywordToken : IPrimitiveTypeToken { }
    public partial interface IUInt64KeywordToken : IPrimitiveTypeToken { }
    public partial interface ISizeKeywordToken : IPrimitiveTypeToken { }
    public partial interface IOffsetKeywordToken : IPrimitiveTypeToken { }

    public partial interface IFloat32KeywordToken : IPrimitiveTypeToken { }
    public partial interface IFloatKeywordToken : IPrimitiveTypeToken { }
}
