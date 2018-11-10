using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public static partial class TokenTypes
    {
        [NotNull, ItemNotNull]
        private static readonly IReadOnlyList<Type> Keyword = new List<Type>()
        {
            typeof(PublicKeywordToken),
            typeof(ProtectedKeywordToken),
            typeof(PrivateKeywordToken),
            typeof(InternalKeywordToken),
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
            typeof(WhereKeywordToken),
            typeof(ConstKeywordToken),
            typeof(AliasKeywordToken),
            typeof(UninitializedKeywordToken),
            typeof(NoneKeywordToken),
            typeof(OperatorKeywordToken),
            typeof(ImplicitKeywordToken),
            typeof(ExplicitKeywordToken),
            typeof(MoveKeywordToken),
            typeof(CopyKeywordToken),
            typeof(MatchKeywordToken),
            typeof(LoopKeywordToken),
            typeof(WhileKeywordToken),
            typeof(BreakKeywordToken),
            typeof(NextKeywordToken),
            typeof(OverrideKeywordToken),
            typeof(AnyKeywordToken),
            typeof(TrueKeywordToken),
            typeof(FalseKeywordToken),
            typeof(AsKeywordToken),
            typeof(AndKeywordToken),
            typeof(OrKeywordToken),
            typeof(XorKeywordToken),
            typeof(NotKeywordToken),
            typeof(TraitKeywordToken),
        }.AsReadOnly().NotNull();
    }

    public static partial class TokenFactory
    {
        [NotNull]
        public static IPublicKeywordToken PublicKeyword(TextSpan span)
        {
            return new PublicKeywordToken(span);
        }

        [NotNull]
        public static IProtectedKeywordToken ProtectedKeyword(TextSpan span)
        {
            return new ProtectedKeywordToken(span);
        }

        [NotNull]
        public static IPrivateKeywordToken PrivateKeyword(TextSpan span)
        {
            return new PrivateKeywordToken(span);
        }

        [NotNull]
        public static IInternalKeywordToken InternalKeyword(TextSpan span)
        {
            return new InternalKeywordToken(span);
        }

        [NotNull]
        public static ILetKeywordToken LetKeyword(TextSpan span)
        {
            return new LetKeywordToken(span);
        }

        [NotNull]
        public static IVarKeywordToken VarKeyword(TextSpan span)
        {
            return new VarKeywordToken(span);
        }

        [NotNull]
        public static IVoidKeywordToken VoidKeyword(TextSpan span)
        {
            return new VoidKeywordToken(span);
        }

        [NotNull]
        public static IIntKeywordToken IntKeyword(TextSpan span)
        {
            return new IntKeywordToken(span);
        }

        [NotNull]
        public static IUIntKeywordToken UIntKeyword(TextSpan span)
        {
            return new UIntKeywordToken(span);
        }

        [NotNull]
        public static IByteKeywordToken ByteKeyword(TextSpan span)
        {
            return new ByteKeywordToken(span);
        }

        [NotNull]
        public static ISizeKeywordToken SizeKeyword(TextSpan span)
        {
            return new SizeKeywordToken(span);
        }

        [NotNull]
        public static IBoolKeywordToken BoolKeyword(TextSpan span)
        {
            return new BoolKeywordToken(span);
        }

        [NotNull]
        public static IStringKeywordToken StringKeyword(TextSpan span)
        {
            return new StringKeywordToken(span);
        }

        [NotNull]
        public static INeverKeywordToken NeverKeyword(TextSpan span)
        {
            return new NeverKeywordToken(span);
        }

        [NotNull]
        public static IReturnKeywordToken ReturnKeyword(TextSpan span)
        {
            return new ReturnKeywordToken(span);
        }

        [NotNull]
        public static IClassKeywordToken ClassKeyword(TextSpan span)
        {
            return new ClassKeywordToken(span);
        }

        [NotNull]
        public static IFunctionKeywordToken FunctionKeyword(TextSpan span)
        {
            return new FunctionKeywordToken(span);
        }

        [NotNull]
        public static INewKeywordToken NewKeyword(TextSpan span)
        {
            return new NewKeywordToken(span);
        }

        [NotNull]
        public static IInitKeywordToken InitKeyword(TextSpan span)
        {
            return new InitKeywordToken(span);
        }

        [NotNull]
        public static IDeleteKeywordToken DeleteKeyword(TextSpan span)
        {
            return new DeleteKeywordToken(span);
        }

        [NotNull]
        public static IOwnedKeywordToken OwnedKeyword(TextSpan span)
        {
            return new OwnedKeywordToken(span);
        }

        [NotNull]
        public static INamespaceKeywordToken NamespaceKeyword(TextSpan span)
        {
            return new NamespaceKeywordToken(span);
        }

        [NotNull]
        public static IUsingKeywordToken UsingKeyword(TextSpan span)
        {
            return new UsingKeywordToken(span);
        }

        [NotNull]
        public static IForeachKeywordToken ForeachKeyword(TextSpan span)
        {
            return new ForeachKeywordToken(span);
        }

        [NotNull]
        public static IInKeywordToken InKeyword(TextSpan span)
        {
            return new InKeywordToken(span);
        }

        [NotNull]
        public static IIfKeywordToken IfKeyword(TextSpan span)
        {
            return new IfKeywordToken(span);
        }

        [NotNull]
        public static IElseKeywordToken ElseKeyword(TextSpan span)
        {
            return new ElseKeywordToken(span);
        }

        [NotNull]
        public static IStructKeywordToken StructKeyword(TextSpan span)
        {
            return new StructKeywordToken(span);
        }

        [NotNull]
        public static IEnumKeywordToken EnumKeyword(TextSpan span)
        {
            return new EnumKeywordToken(span);
        }

        [NotNull]
        public static IUnsafeKeywordToken UnsafeKeyword(TextSpan span)
        {
            return new UnsafeKeywordToken(span);
        }

        [NotNull]
        public static ISafeKeywordToken SafeKeyword(TextSpan span)
        {
            return new SafeKeywordToken(span);
        }

        [NotNull]
        public static ISelfKeywordToken SelfKeyword(TextSpan span)
        {
            return new SelfKeywordToken(span);
        }

        [NotNull]
        public static ISelfTypeKeywordToken SelfTypeKeyword(TextSpan span)
        {
            return new SelfTypeKeywordToken(span);
        }

        [NotNull]
        public static IBaseKeywordToken BaseKeyword(TextSpan span)
        {
            return new BaseKeywordToken(span);
        }

        [NotNull]
        public static IExtendKeywordToken ExtendKeyword(TextSpan span)
        {
            return new ExtendKeywordToken(span);
        }

        [NotNull]
        public static ITypeKeywordToken TypeKeyword(TextSpan span)
        {
            return new TypeKeywordToken(span);
        }

        [NotNull]
        public static IMetatypeKeywordToken MetatypeKeyword(TextSpan span)
        {
            return new MetatypeKeywordToken(span);
        }

        [NotNull]
        public static IMutableKeywordToken MutableKeyword(TextSpan span)
        {
            return new MutableKeywordToken(span);
        }

        [NotNull]
        public static IParamsKeywordToken ParamsKeyword(TextSpan span)
        {
            return new ParamsKeywordToken(span);
        }

        [NotNull]
        public static IMayKeywordToken MayKeyword(TextSpan span)
        {
            return new MayKeywordToken(span);
        }

        [NotNull]
        public static INoKeywordToken NoKeyword(TextSpan span)
        {
            return new NoKeywordToken(span);
        }

        [NotNull]
        public static IThrowKeywordToken ThrowKeyword(TextSpan span)
        {
            return new ThrowKeywordToken(span);
        }

        [NotNull]
        public static IRefKeywordToken RefKeyword(TextSpan span)
        {
            return new RefKeywordToken(span);
        }

        [NotNull]
        public static IAbstractKeywordToken AbstractKeyword(TextSpan span)
        {
            return new AbstractKeywordToken(span);
        }

        [NotNull]
        public static IGetKeywordToken GetKeyword(TextSpan span)
        {
            return new GetKeywordToken(span);
        }

        [NotNull]
        public static ISetKeywordToken SetKeyword(TextSpan span)
        {
            return new SetKeywordToken(span);
        }

        [NotNull]
        public static IRequiresKeywordToken RequiresKeyword(TextSpan span)
        {
            return new RequiresKeywordToken(span);
        }

        [NotNull]
        public static IEnsuresKeywordToken EnsuresKeyword(TextSpan span)
        {
            return new EnsuresKeywordToken(span);
        }

        [NotNull]
        public static IInvariantKeywordToken InvariantKeyword(TextSpan span)
        {
            return new InvariantKeywordToken(span);
        }

        [NotNull]
        public static IWhereKeywordToken WhereKeyword(TextSpan span)
        {
            return new WhereKeywordToken(span);
        }

        [NotNull]
        public static IConstKeywordToken ConstKeyword(TextSpan span)
        {
            return new ConstKeywordToken(span);
        }

        [NotNull]
        public static IAliasKeywordToken AliasKeyword(TextSpan span)
        {
            return new AliasKeywordToken(span);
        }

        [NotNull]
        public static IUninitializedKeywordToken UninitializedKeyword(TextSpan span)
        {
            return new UninitializedKeywordToken(span);
        }

        [NotNull]
        public static INoneKeywordToken NoneKeyword(TextSpan span)
        {
            return new NoneKeywordToken(span);
        }

        [NotNull]
        public static IOperatorKeywordToken OperatorKeyword(TextSpan span)
        {
            return new OperatorKeywordToken(span);
        }

        [NotNull]
        public static IImplicitKeywordToken ImplicitKeyword(TextSpan span)
        {
            return new ImplicitKeywordToken(span);
        }

        [NotNull]
        public static IExplicitKeywordToken ExplicitKeyword(TextSpan span)
        {
            return new ExplicitKeywordToken(span);
        }

        [NotNull]
        public static IMoveKeywordToken MoveKeyword(TextSpan span)
        {
            return new MoveKeywordToken(span);
        }

        [NotNull]
        public static ICopyKeywordToken CopyKeyword(TextSpan span)
        {
            return new CopyKeywordToken(span);
        }

        [NotNull]
        public static IMatchKeywordToken MatchKeyword(TextSpan span)
        {
            return new MatchKeywordToken(span);
        }

        [NotNull]
        public static ILoopKeywordToken LoopKeyword(TextSpan span)
        {
            return new LoopKeywordToken(span);
        }

        [NotNull]
        public static IWhileKeywordToken WhileKeyword(TextSpan span)
        {
            return new WhileKeywordToken(span);
        }

        [NotNull]
        public static IBreakKeywordToken BreakKeyword(TextSpan span)
        {
            return new BreakKeywordToken(span);
        }

        [NotNull]
        public static INextKeywordToken NextKeyword(TextSpan span)
        {
            return new NextKeywordToken(span);
        }

        [NotNull]
        public static IOverrideKeywordToken OverrideKeyword(TextSpan span)
        {
            return new OverrideKeywordToken(span);
        }

        [NotNull]
        public static IAnyKeywordToken AnyKeyword(TextSpan span)
        {
            return new AnyKeywordToken(span);
        }

        [NotNull]
        public static ITrueKeywordToken TrueKeyword(TextSpan span)
        {
            return new TrueKeywordToken(span);
        }

        [NotNull]
        public static IFalseKeywordToken FalseKeyword(TextSpan span)
        {
            return new FalseKeywordToken(span);
        }

        [NotNull]
        public static IAsKeywordToken AsKeyword(TextSpan span)
        {
            return new AsKeywordToken(span);
        }

        [NotNull]
        public static IAndKeywordToken AndKeyword(TextSpan span)
        {
            return new AndKeywordToken(span);
        }

        [NotNull]
        public static IOrKeywordToken OrKeyword(TextSpan span)
        {
            return new OrKeywordToken(span);
        }

        [NotNull]
        public static IXorKeywordToken XorKeyword(TextSpan span)
        {
            return new XorKeywordToken(span);
        }

        [NotNull]
        public static INotKeywordToken NotKeyword(TextSpan span)
        {
            return new NotKeywordToken(span);
        }

        [NotNull]
        public static ITraitKeywordToken TraitKeyword(TextSpan span)
        {
            return new TraitKeywordToken(span);
        }

    }

    public partial interface IPublicKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IPublicKeywordToken : IKeywordToken, IPublicKeywordTokenPlace { }
    internal partial class PublicKeywordToken : Token, IPublicKeywordToken
    {
        public PublicKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IProtectedKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IProtectedKeywordToken : IKeywordToken, IProtectedKeywordTokenPlace { }
    internal partial class ProtectedKeywordToken : Token, IProtectedKeywordToken
    {
        public ProtectedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPrivateKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IPrivateKeywordToken : IKeywordToken, IPrivateKeywordTokenPlace { }
    internal partial class PrivateKeywordToken : Token, IPrivateKeywordToken
    {
        public PrivateKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInternalKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IInternalKeywordToken : IKeywordToken, IInternalKeywordTokenPlace { }
    internal partial class InternalKeywordToken : Token, IInternalKeywordToken
    {
        public InternalKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILetKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ILetKeywordToken : IKeywordToken, ILetKeywordTokenPlace { }
    internal partial class LetKeywordToken : Token, ILetKeywordToken
    {
        public LetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IVarKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IVarKeywordToken : IKeywordToken, IVarKeywordTokenPlace { }
    internal partial class VarKeywordToken : Token, IVarKeywordToken
    {
        public VarKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IVoidKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IVoidKeywordToken : IKeywordToken, IVoidKeywordTokenPlace { }
    internal partial class VoidKeywordToken : Token, IVoidKeywordToken
    {
        public VoidKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IIntKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IIntKeywordToken : IKeywordToken, IIntKeywordTokenPlace { }
    internal partial class IntKeywordToken : Token, IIntKeywordToken
    {
        public IntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUIntKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IUIntKeywordToken : IKeywordToken, IUIntKeywordTokenPlace { }
    internal partial class UIntKeywordToken : Token, IUIntKeywordToken
    {
        public UIntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IByteKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IByteKeywordToken : IKeywordToken, IByteKeywordTokenPlace { }
    internal partial class ByteKeywordToken : Token, IByteKeywordToken
    {
        public ByteKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISizeKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ISizeKeywordToken : IKeywordToken, ISizeKeywordTokenPlace { }
    internal partial class SizeKeywordToken : Token, ISizeKeywordToken
    {
        public SizeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IBoolKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IBoolKeywordToken : IKeywordToken, IBoolKeywordTokenPlace { }
    internal partial class BoolKeywordToken : Token, IBoolKeywordToken
    {
        public BoolKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IStringKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IStringKeywordToken : IKeywordToken, IStringKeywordTokenPlace { }
    internal partial class StringKeywordToken : Token, IStringKeywordToken
    {
        public StringKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INeverKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface INeverKeywordToken : IKeywordToken, INeverKeywordTokenPlace { }
    internal partial class NeverKeywordToken : Token, INeverKeywordToken
    {
        public NeverKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IReturnKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IReturnKeywordToken : IKeywordToken, IReturnKeywordTokenPlace { }
    internal partial class ReturnKeywordToken : Token, IReturnKeywordToken
    {
        public ReturnKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IClassKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IClassKeywordToken : IKeywordToken, IClassKeywordTokenPlace { }
    internal partial class ClassKeywordToken : Token, IClassKeywordToken
    {
        public ClassKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IFunctionKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IFunctionKeywordToken : IKeywordToken, IFunctionKeywordTokenPlace { }
    internal partial class FunctionKeywordToken : Token, IFunctionKeywordToken
    {
        public FunctionKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INewKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface INewKeywordToken : IKeywordToken, INewKeywordTokenPlace { }
    internal partial class NewKeywordToken : Token, INewKeywordToken
    {
        public NewKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInitKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IInitKeywordToken : IKeywordToken, IInitKeywordTokenPlace { }
    internal partial class InitKeywordToken : Token, IInitKeywordToken
    {
        public InitKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDeleteKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IDeleteKeywordToken : IKeywordToken, IDeleteKeywordTokenPlace { }
    internal partial class DeleteKeywordToken : Token, IDeleteKeywordToken
    {
        public DeleteKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOwnedKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IOwnedKeywordToken : IKeywordToken, IOwnedKeywordTokenPlace { }
    internal partial class OwnedKeywordToken : Token, IOwnedKeywordToken
    {
        public OwnedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INamespaceKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface INamespaceKeywordToken : IKeywordToken, INamespaceKeywordTokenPlace { }
    internal partial class NamespaceKeywordToken : Token, INamespaceKeywordToken
    {
        public NamespaceKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUsingKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IUsingKeywordToken : IKeywordToken, IUsingKeywordTokenPlace { }
    internal partial class UsingKeywordToken : Token, IUsingKeywordToken
    {
        public UsingKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IForeachKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IForeachKeywordToken : IKeywordToken, IForeachKeywordTokenPlace { }
    internal partial class ForeachKeywordToken : Token, IForeachKeywordToken
    {
        public ForeachKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IInKeywordToken : IKeywordToken, IInKeywordTokenPlace { }
    internal partial class InKeywordToken : Token, IInKeywordToken
    {
        public InKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IIfKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IIfKeywordToken : IKeywordToken, IIfKeywordTokenPlace { }
    internal partial class IfKeywordToken : Token, IIfKeywordToken
    {
        public IfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IElseKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IElseKeywordToken : IKeywordToken, IElseKeywordTokenPlace { }
    internal partial class ElseKeywordToken : Token, IElseKeywordToken
    {
        public ElseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IStructKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IStructKeywordToken : IKeywordToken, IStructKeywordTokenPlace { }
    internal partial class StructKeywordToken : Token, IStructKeywordToken
    {
        public StructKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEnumKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IEnumKeywordToken : IKeywordToken, IEnumKeywordTokenPlace { }
    internal partial class EnumKeywordToken : Token, IEnumKeywordToken
    {
        public EnumKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUnsafeKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IUnsafeKeywordToken : IKeywordToken, IUnsafeKeywordTokenPlace { }
    internal partial class UnsafeKeywordToken : Token, IUnsafeKeywordToken
    {
        public UnsafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISafeKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ISafeKeywordToken : IKeywordToken, ISafeKeywordTokenPlace { }
    internal partial class SafeKeywordToken : Token, ISafeKeywordToken
    {
        public SafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISelfKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ISelfKeywordToken : IKeywordToken, ISelfKeywordTokenPlace { }
    internal partial class SelfKeywordToken : Token, ISelfKeywordToken
    {
        public SelfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISelfTypeKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ISelfTypeKeywordToken : IKeywordToken, ISelfTypeKeywordTokenPlace { }
    internal partial class SelfTypeKeywordToken : Token, ISelfTypeKeywordToken
    {
        public SelfTypeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IBaseKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IBaseKeywordToken : IKeywordToken, IBaseKeywordTokenPlace { }
    internal partial class BaseKeywordToken : Token, IBaseKeywordToken
    {
        public BaseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IExtendKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IExtendKeywordToken : IKeywordToken, IExtendKeywordTokenPlace { }
    internal partial class ExtendKeywordToken : Token, IExtendKeywordToken
    {
        public ExtendKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ITypeKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ITypeKeywordToken : IKeywordToken, ITypeKeywordTokenPlace { }
    internal partial class TypeKeywordToken : Token, ITypeKeywordToken
    {
        public TypeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMetatypeKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IMetatypeKeywordToken : IKeywordToken, IMetatypeKeywordTokenPlace { }
    internal partial class MetatypeKeywordToken : Token, IMetatypeKeywordToken
    {
        public MetatypeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMutableKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IMutableKeywordToken : IKeywordToken, IMutableKeywordTokenPlace { }
    internal partial class MutableKeywordToken : Token, IMutableKeywordToken
    {
        public MutableKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IParamsKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IParamsKeywordToken : IKeywordToken, IParamsKeywordTokenPlace { }
    internal partial class ParamsKeywordToken : Token, IParamsKeywordToken
    {
        public ParamsKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMayKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IMayKeywordToken : IKeywordToken, IMayKeywordTokenPlace { }
    internal partial class MayKeywordToken : Token, IMayKeywordToken
    {
        public MayKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INoKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface INoKeywordToken : IKeywordToken, INoKeywordTokenPlace { }
    internal partial class NoKeywordToken : Token, INoKeywordToken
    {
        public NoKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IThrowKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IThrowKeywordToken : IKeywordToken, IThrowKeywordTokenPlace { }
    internal partial class ThrowKeywordToken : Token, IThrowKeywordToken
    {
        public ThrowKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IRefKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IRefKeywordToken : IKeywordToken, IRefKeywordTokenPlace { }
    internal partial class RefKeywordToken : Token, IRefKeywordToken
    {
        public RefKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAbstractKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IAbstractKeywordToken : IKeywordToken, IAbstractKeywordTokenPlace { }
    internal partial class AbstractKeywordToken : Token, IAbstractKeywordToken
    {
        public AbstractKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGetKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IGetKeywordToken : IKeywordToken, IGetKeywordTokenPlace { }
    internal partial class GetKeywordToken : Token, IGetKeywordToken
    {
        public GetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISetKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ISetKeywordToken : IKeywordToken, ISetKeywordTokenPlace { }
    internal partial class SetKeywordToken : Token, ISetKeywordToken
    {
        public SetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IRequiresKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IRequiresKeywordToken : IKeywordToken, IRequiresKeywordTokenPlace { }
    internal partial class RequiresKeywordToken : Token, IRequiresKeywordToken
    {
        public RequiresKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEnsuresKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IEnsuresKeywordToken : IKeywordToken, IEnsuresKeywordTokenPlace { }
    internal partial class EnsuresKeywordToken : Token, IEnsuresKeywordToken
    {
        public EnsuresKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInvariantKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IInvariantKeywordToken : IKeywordToken, IInvariantKeywordTokenPlace { }
    internal partial class InvariantKeywordToken : Token, IInvariantKeywordToken
    {
        public InvariantKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IWhereKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IWhereKeywordToken : IKeywordToken, IWhereKeywordTokenPlace { }
    internal partial class WhereKeywordToken : Token, IWhereKeywordToken
    {
        public WhereKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IConstKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IConstKeywordToken : IKeywordToken, IConstKeywordTokenPlace { }
    internal partial class ConstKeywordToken : Token, IConstKeywordToken
    {
        public ConstKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAliasKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IAliasKeywordToken : IKeywordToken, IAliasKeywordTokenPlace { }
    internal partial class AliasKeywordToken : Token, IAliasKeywordToken
    {
        public AliasKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUninitializedKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IUninitializedKeywordToken : IKeywordToken, IUninitializedKeywordTokenPlace { }
    internal partial class UninitializedKeywordToken : Token, IUninitializedKeywordToken
    {
        public UninitializedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INoneKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface INoneKeywordToken : IKeywordToken, INoneKeywordTokenPlace { }
    internal partial class NoneKeywordToken : Token, INoneKeywordToken
    {
        public NoneKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOperatorKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IOperatorKeywordToken : IKeywordToken, IOperatorKeywordTokenPlace { }
    internal partial class OperatorKeywordToken : Token, IOperatorKeywordToken
    {
        public OperatorKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IImplicitKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IImplicitKeywordToken : IKeywordToken, IImplicitKeywordTokenPlace { }
    internal partial class ImplicitKeywordToken : Token, IImplicitKeywordToken
    {
        public ImplicitKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IExplicitKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IExplicitKeywordToken : IKeywordToken, IExplicitKeywordTokenPlace { }
    internal partial class ExplicitKeywordToken : Token, IExplicitKeywordToken
    {
        public ExplicitKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMoveKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IMoveKeywordToken : IKeywordToken, IMoveKeywordTokenPlace { }
    internal partial class MoveKeywordToken : Token, IMoveKeywordToken
    {
        public MoveKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICopyKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ICopyKeywordToken : IKeywordToken, ICopyKeywordTokenPlace { }
    internal partial class CopyKeywordToken : Token, ICopyKeywordToken
    {
        public CopyKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMatchKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IMatchKeywordToken : IKeywordToken, IMatchKeywordTokenPlace { }
    internal partial class MatchKeywordToken : Token, IMatchKeywordToken
    {
        public MatchKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILoopKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ILoopKeywordToken : IKeywordToken, ILoopKeywordTokenPlace { }
    internal partial class LoopKeywordToken : Token, ILoopKeywordToken
    {
        public LoopKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IWhileKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IWhileKeywordToken : IKeywordToken, IWhileKeywordTokenPlace { }
    internal partial class WhileKeywordToken : Token, IWhileKeywordToken
    {
        public WhileKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IBreakKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IBreakKeywordToken : IKeywordToken, IBreakKeywordTokenPlace { }
    internal partial class BreakKeywordToken : Token, IBreakKeywordToken
    {
        public BreakKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INextKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface INextKeywordToken : IKeywordToken, INextKeywordTokenPlace { }
    internal partial class NextKeywordToken : Token, INextKeywordToken
    {
        public NextKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOverrideKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IOverrideKeywordToken : IKeywordToken, IOverrideKeywordTokenPlace { }
    internal partial class OverrideKeywordToken : Token, IOverrideKeywordToken
    {
        public OverrideKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAnyKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IAnyKeywordToken : IKeywordToken, IAnyKeywordTokenPlace { }
    internal partial class AnyKeywordToken : Token, IAnyKeywordToken
    {
        public AnyKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ITrueKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ITrueKeywordToken : IKeywordToken, ITrueKeywordTokenPlace { }
    internal partial class TrueKeywordToken : Token, ITrueKeywordToken
    {
        public TrueKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IFalseKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IFalseKeywordToken : IKeywordToken, IFalseKeywordTokenPlace { }
    internal partial class FalseKeywordToken : Token, IFalseKeywordToken
    {
        public FalseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IAsKeywordToken : IKeywordToken, IAsKeywordTokenPlace { }
    internal partial class AsKeywordToken : Token, IAsKeywordToken
    {
        public AsKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAndKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IAndKeywordToken : IKeywordToken, IAndKeywordTokenPlace { }
    internal partial class AndKeywordToken : Token, IAndKeywordToken
    {
        public AndKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOrKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IOrKeywordToken : IKeywordToken, IOrKeywordTokenPlace { }
    internal partial class OrKeywordToken : Token, IOrKeywordToken
    {
        public OrKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IXorKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface IXorKeywordToken : IKeywordToken, IXorKeywordTokenPlace { }
    internal partial class XorKeywordToken : Token, IXorKeywordToken
    {
        public XorKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INotKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface INotKeywordToken : IKeywordToken, INotKeywordTokenPlace { }
    internal partial class NotKeywordToken : Token, INotKeywordToken
    {
        public NotKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ITraitKeywordTokenPlace : IKeywordTokenPlace { }
    public partial interface ITraitKeywordToken : IKeywordToken, ITraitKeywordTokenPlace { }
    internal partial class TraitKeywordToken : Token, ITraitKeywordToken
    {
        public TraitKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
    public partial interface IMissingToken :
        IPublicKeywordTokenPlace,
        IProtectedKeywordTokenPlace,
        IPrivateKeywordTokenPlace,
        IInternalKeywordTokenPlace,
        ILetKeywordTokenPlace,
        IVarKeywordTokenPlace,
        IVoidKeywordTokenPlace,
        IIntKeywordTokenPlace,
        IUIntKeywordTokenPlace,
        IByteKeywordTokenPlace,
        ISizeKeywordTokenPlace,
        IBoolKeywordTokenPlace,
        IStringKeywordTokenPlace,
        INeverKeywordTokenPlace,
        IReturnKeywordTokenPlace,
        IClassKeywordTokenPlace,
        IFunctionKeywordTokenPlace,
        INewKeywordTokenPlace,
        IInitKeywordTokenPlace,
        IDeleteKeywordTokenPlace,
        IOwnedKeywordTokenPlace,
        INamespaceKeywordTokenPlace,
        IUsingKeywordTokenPlace,
        IForeachKeywordTokenPlace,
        IInKeywordTokenPlace,
        IIfKeywordTokenPlace,
        IElseKeywordTokenPlace,
        IStructKeywordTokenPlace,
        IEnumKeywordTokenPlace,
        IUnsafeKeywordTokenPlace,
        ISafeKeywordTokenPlace,
        ISelfKeywordTokenPlace,
        ISelfTypeKeywordTokenPlace,
        IBaseKeywordTokenPlace,
        IExtendKeywordTokenPlace,
        ITypeKeywordTokenPlace,
        IMetatypeKeywordTokenPlace,
        IMutableKeywordTokenPlace,
        IParamsKeywordTokenPlace,
        IMayKeywordTokenPlace,
        INoKeywordTokenPlace,
        IThrowKeywordTokenPlace,
        IRefKeywordTokenPlace,
        IAbstractKeywordTokenPlace,
        IGetKeywordTokenPlace,
        ISetKeywordTokenPlace,
        IRequiresKeywordTokenPlace,
        IEnsuresKeywordTokenPlace,
        IInvariantKeywordTokenPlace,
        IWhereKeywordTokenPlace,
        IConstKeywordTokenPlace,
        IAliasKeywordTokenPlace,
        IUninitializedKeywordTokenPlace,
        INoneKeywordTokenPlace,
        IOperatorKeywordTokenPlace,
        IImplicitKeywordTokenPlace,
        IExplicitKeywordTokenPlace,
        IMoveKeywordTokenPlace,
        ICopyKeywordTokenPlace,
        IMatchKeywordTokenPlace,
        ILoopKeywordTokenPlace,
        IWhileKeywordTokenPlace,
        IBreakKeywordTokenPlace,
        INextKeywordTokenPlace,
        IOverrideKeywordTokenPlace,
        IAnyKeywordTokenPlace,
        ITrueKeywordTokenPlace,
        IFalseKeywordTokenPlace,
        IAsKeywordTokenPlace,
        IAndKeywordTokenPlace,
        IOrKeywordTokenPlace,
        IXorKeywordTokenPlace,
        INotKeywordTokenPlace,
        ITraitKeywordTokenPlace,
        IKeywordTokenPlace // Implied, but saves issues with commas
    { }
}
