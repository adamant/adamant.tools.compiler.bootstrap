using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public static partial class TokenTypes
    {
        [NotNull][ItemNotNull]
        public static readonly IReadOnlyList<Type> Keyword = new List<Type>()
        {
            typeof(PublicKeywordToken),
            typeof(ProtectedKeywordToken),
            typeof(PrivateKeywordToken),
            typeof(LetKeywordToken),
            typeof(VarKeywordToken),
            typeof(VoidKeywordToken),
            typeof(IntKeywordToken),
            typeof(UIntKeywordToken),
            typeof(ByteKeywordToken),
            typeof(SizeKeywordToken),
            typeof(BoolKeywordToken),
            typeof(StringKeywordToken),
            typeof(NeverKeywordToken),
            typeof(ReturnKeywordToken),
            typeof(ClassKeywordToken),
            typeof(FunctionKeywordToken),
            typeof(NewKeywordToken),
            typeof(InitKeywordToken),
            typeof(DeleteKeywordToken),
            typeof(OwnedKeywordToken),
            typeof(NamespaceKeywordToken),
            typeof(UsingKeywordToken),
            typeof(ForeachKeywordToken),
            typeof(InKeywordToken),
            typeof(IfKeywordToken),
            typeof(ElseKeywordToken),
            typeof(StructKeywordToken),
            typeof(EnumKeywordToken),
            typeof(UnsafeKeywordToken),
            typeof(SafeKeywordToken),
            typeof(SelfKeywordToken),
            typeof(SelfTypeKeywordToken),
            typeof(BaseKeywordToken),
            typeof(ExtendKeywordToken),
            typeof(TypeKeywordToken),
            typeof(MetatypeKeywordToken),
            typeof(MutableKeywordToken),
            typeof(ParamsKeywordToken),
            typeof(MayKeywordToken),
            typeof(NoKeywordToken),
            typeof(ThrowKeywordToken),
            typeof(RefKeywordToken),
            typeof(AbstractKeywordToken),
            typeof(GetKeywordToken),
            typeof(SetKeywordToken),
            typeof(RequiresKeywordToken),
            typeof(EnsuresKeywordToken),
            typeof(InvariantKeywordToken),
        }.AsReadOnly().AssertNotNull();
    }

    public partial interface IPublicKeywordToken : IKeywordToken { }
    public class PublicKeywordToken : KeywordToken, IPublicKeywordToken
    {
        public PublicKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IProtectedKeywordToken : IKeywordToken { }
    public class ProtectedKeywordToken : KeywordToken, IProtectedKeywordToken
    {
        public ProtectedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPrivateKeywordToken : IKeywordToken { }
    public class PrivateKeywordToken : KeywordToken, IPrivateKeywordToken
    {
        public PrivateKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILetKeywordToken : IKeywordToken { }
    public class LetKeywordToken : KeywordToken, ILetKeywordToken
    {
        public LetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IVarKeywordToken : IKeywordToken { }
    public class VarKeywordToken : KeywordToken, IVarKeywordToken
    {
        public VarKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IVoidKeywordToken : IKeywordToken { }
    public class VoidKeywordToken : KeywordToken, IVoidKeywordToken
    {
        public VoidKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IIntKeywordToken : IKeywordToken { }
    public class IntKeywordToken : KeywordToken, IIntKeywordToken
    {
        public IntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUIntKeywordToken : IKeywordToken { }
    public class UIntKeywordToken : KeywordToken, IUIntKeywordToken
    {
        public UIntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IByteKeywordToken : IKeywordToken { }
    public class ByteKeywordToken : KeywordToken, IByteKeywordToken
    {
        public ByteKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISizeKeywordToken : IKeywordToken { }
    public class SizeKeywordToken : KeywordToken, ISizeKeywordToken
    {
        public SizeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IBoolKeywordToken : IKeywordToken { }
    public class BoolKeywordToken : KeywordToken, IBoolKeywordToken
    {
        public BoolKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IStringKeywordToken : IKeywordToken { }
    public class StringKeywordToken : KeywordToken, IStringKeywordToken
    {
        public StringKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INeverKeywordToken : IKeywordToken { }
    public class NeverKeywordToken : KeywordToken, INeverKeywordToken
    {
        public NeverKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IReturnKeywordToken : IKeywordToken { }
    public class ReturnKeywordToken : KeywordToken, IReturnKeywordToken
    {
        public ReturnKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IClassKeywordToken : IKeywordToken { }
    public class ClassKeywordToken : KeywordToken, IClassKeywordToken
    {
        public ClassKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IFunctionKeywordToken : IKeywordToken { }
    public class FunctionKeywordToken : KeywordToken, IFunctionKeywordToken
    {
        public FunctionKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INewKeywordToken : IKeywordToken { }
    public class NewKeywordToken : KeywordToken, INewKeywordToken
    {
        public NewKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInitKeywordToken : IKeywordToken { }
    public class InitKeywordToken : KeywordToken, IInitKeywordToken
    {
        public InitKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDeleteKeywordToken : IKeywordToken { }
    public class DeleteKeywordToken : KeywordToken, IDeleteKeywordToken
    {
        public DeleteKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOwnedKeywordToken : IKeywordToken { }
    public class OwnedKeywordToken : KeywordToken, IOwnedKeywordToken
    {
        public OwnedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INamespaceKeywordToken : IKeywordToken { }
    public class NamespaceKeywordToken : KeywordToken, INamespaceKeywordToken
    {
        public NamespaceKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUsingKeywordToken : IKeywordToken { }
    public class UsingKeywordToken : KeywordToken, IUsingKeywordToken
    {
        public UsingKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IForeachKeywordToken : IKeywordToken { }
    public class ForeachKeywordToken : KeywordToken, IForeachKeywordToken
    {
        public ForeachKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInKeywordToken : IKeywordToken { }
    public class InKeywordToken : KeywordToken, IInKeywordToken
    {
        public InKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IIfKeywordToken : IKeywordToken { }
    public class IfKeywordToken : KeywordToken, IIfKeywordToken
    {
        public IfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IElseKeywordToken : IKeywordToken { }
    public class ElseKeywordToken : KeywordToken, IElseKeywordToken
    {
        public ElseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IStructKeywordToken : IKeywordToken { }
    public class StructKeywordToken : KeywordToken, IStructKeywordToken
    {
        public StructKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEnumKeywordToken : IKeywordToken { }
    public class EnumKeywordToken : KeywordToken, IEnumKeywordToken
    {
        public EnumKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUnsafeKeywordToken : IKeywordToken { }
    public class UnsafeKeywordToken : KeywordToken, IUnsafeKeywordToken
    {
        public UnsafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISafeKeywordToken : IKeywordToken { }
    public class SafeKeywordToken : KeywordToken, ISafeKeywordToken
    {
        public SafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISelfKeywordToken : IKeywordToken { }
    public class SelfKeywordToken : KeywordToken, ISelfKeywordToken
    {
        public SelfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISelfTypeKeywordToken : IKeywordToken { }
    public class SelfTypeKeywordToken : KeywordToken, ISelfTypeKeywordToken
    {
        public SelfTypeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IBaseKeywordToken : IKeywordToken { }
    public class BaseKeywordToken : KeywordToken, IBaseKeywordToken
    {
        public BaseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IExtendKeywordToken : IKeywordToken { }
    public class ExtendKeywordToken : KeywordToken, IExtendKeywordToken
    {
        public ExtendKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ITypeKeywordToken : IKeywordToken { }
    public class TypeKeywordToken : KeywordToken, ITypeKeywordToken
    {
        public TypeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMetatypeKeywordToken : IKeywordToken { }
    public class MetatypeKeywordToken : KeywordToken, IMetatypeKeywordToken
    {
        public MetatypeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMutableKeywordToken : IKeywordToken { }
    public class MutableKeywordToken : KeywordToken, IMutableKeywordToken
    {
        public MutableKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IParamsKeywordToken : IKeywordToken { }
    public class ParamsKeywordToken : KeywordToken, IParamsKeywordToken
    {
        public ParamsKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMayKeywordToken : IKeywordToken { }
    public class MayKeywordToken : KeywordToken, IMayKeywordToken
    {
        public MayKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INoKeywordToken : IKeywordToken { }
    public class NoKeywordToken : KeywordToken, INoKeywordToken
    {
        public NoKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IThrowKeywordToken : IKeywordToken { }
    public class ThrowKeywordToken : KeywordToken, IThrowKeywordToken
    {
        public ThrowKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IRefKeywordToken : IKeywordToken { }
    public class RefKeywordToken : KeywordToken, IRefKeywordToken
    {
        public RefKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAbstractKeywordToken : IKeywordToken { }
    public class AbstractKeywordToken : KeywordToken, IAbstractKeywordToken
    {
        public AbstractKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGetKeywordToken : IKeywordToken { }
    public class GetKeywordToken : KeywordToken, IGetKeywordToken
    {
        public GetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISetKeywordToken : IKeywordToken { }
    public class SetKeywordToken : KeywordToken, ISetKeywordToken
    {
        public SetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IRequiresKeywordToken : IKeywordToken { }
    public class RequiresKeywordToken : KeywordToken, IRequiresKeywordToken
    {
        public RequiresKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEnsuresKeywordToken : IKeywordToken { }
    public class EnsuresKeywordToken : KeywordToken, IEnsuresKeywordToken
    {
        public EnsuresKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInvariantKeywordToken : IKeywordToken { }
    public class InvariantKeywordToken : KeywordToken, IInvariantKeywordToken
    {
        public InvariantKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
    public partial class MissingToken :
        IPublicKeywordToken,
        IProtectedKeywordToken,
        IPrivateKeywordToken,
        ILetKeywordToken,
        IVarKeywordToken,
        IVoidKeywordToken,
        IIntKeywordToken,
        IUIntKeywordToken,
        IByteKeywordToken,
        ISizeKeywordToken,
        IBoolKeywordToken,
        IStringKeywordToken,
        INeverKeywordToken,
        IReturnKeywordToken,
        IClassKeywordToken,
        IFunctionKeywordToken,
        INewKeywordToken,
        IInitKeywordToken,
        IDeleteKeywordToken,
        IOwnedKeywordToken,
        INamespaceKeywordToken,
        IUsingKeywordToken,
        IForeachKeywordToken,
        IInKeywordToken,
        IIfKeywordToken,
        IElseKeywordToken,
        IStructKeywordToken,
        IEnumKeywordToken,
        IUnsafeKeywordToken,
        ISafeKeywordToken,
        ISelfKeywordToken,
        ISelfTypeKeywordToken,
        IBaseKeywordToken,
        IExtendKeywordToken,
        ITypeKeywordToken,
        IMetatypeKeywordToken,
        IMutableKeywordToken,
        IParamsKeywordToken,
        IMayKeywordToken,
        INoKeywordToken,
        IThrowKeywordToken,
        IRefKeywordToken,
        IAbstractKeywordToken,
        IGetKeywordToken,
        ISetKeywordToken,
        IRequiresKeywordToken,
        IEnsuresKeywordToken,
        IInvariantKeywordToken,
        IKeywordToken // Implied, but saves issues with commas
    { }
}
