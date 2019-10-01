using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public static partial class TokenTypes
    {
        private static readonly IReadOnlyList<Type> Keyword = new List<Type>()
        {
            typeof(PublishedKeywordToken),
            typeof(PublicKeywordToken),
            typeof(LetKeywordToken),
            typeof(VarKeywordToken),
            typeof(VoidKeywordToken),
            typeof(IntKeywordToken),
            typeof(UIntKeywordToken),
            typeof(ByteKeywordToken),
            typeof(SizeKeywordToken),
            typeof(OffsetKeywordToken),
            typeof(BoolKeywordToken),
            typeof(NeverKeywordToken),
            typeof(ReturnKeywordToken),
            typeof(ClassKeywordToken),
            typeof(FunctionKeywordToken),
            typeof(NewKeywordToken),
            typeof(OwnedKeywordToken),
            typeof(ForeverKeywordToken),
            typeof(NamespaceKeywordToken),
            typeof(UsingKeywordToken),
            typeof(ForeachKeywordToken),
            typeof(InKeywordToken),
            typeof(IfKeywordToken),
            typeof(ElseKeywordToken),
            typeof(UnsafeKeywordToken),
            typeof(SafeKeywordToken),
            typeof(SelfKeywordToken),
            typeof(MutableKeywordToken),
            typeof(NoneKeywordToken),
            typeof(MoveKeywordToken),
            typeof(CopyKeywordToken),
            typeof(LoopKeywordToken),
            typeof(WhileKeywordToken),
            typeof(BreakKeywordToken),
            typeof(NextKeywordToken),
            typeof(AnyKeywordToken),
            typeof(TrueKeywordToken),
            typeof(FalseKeywordToken),
            typeof(AsKeywordToken),
            typeof(AndKeywordToken),
            typeof(OrKeywordToken),
            typeof(NotKeywordToken),
        }.AsReadOnly();
    }

    public static partial class TokenFactory
    {
        public static IPublishedKeywordToken PublishedKeyword(TextSpan span)
        {
            return new PublishedKeywordToken(span);
        }
        public static IPublicKeywordToken PublicKeyword(TextSpan span)
        {
            return new PublicKeywordToken(span);
        }
        public static ILetKeywordToken LetKeyword(TextSpan span)
        {
            return new LetKeywordToken(span);
        }
        public static IVarKeywordToken VarKeyword(TextSpan span)
        {
            return new VarKeywordToken(span);
        }
        public static IVoidKeywordToken VoidKeyword(TextSpan span)
        {
            return new VoidKeywordToken(span);
        }
        public static IIntKeywordToken IntKeyword(TextSpan span)
        {
            return new IntKeywordToken(span);
        }
        public static IUIntKeywordToken UIntKeyword(TextSpan span)
        {
            return new UIntKeywordToken(span);
        }
        public static IByteKeywordToken ByteKeyword(TextSpan span)
        {
            return new ByteKeywordToken(span);
        }
        public static ISizeKeywordToken SizeKeyword(TextSpan span)
        {
            return new SizeKeywordToken(span);
        }
        public static IOffsetKeywordToken OffsetKeyword(TextSpan span)
        {
            return new OffsetKeywordToken(span);
        }
        public static IBoolKeywordToken BoolKeyword(TextSpan span)
        {
            return new BoolKeywordToken(span);
        }
        public static INeverKeywordToken NeverKeyword(TextSpan span)
        {
            return new NeverKeywordToken(span);
        }
        public static IReturnKeywordToken ReturnKeyword(TextSpan span)
        {
            return new ReturnKeywordToken(span);
        }
        public static IClassKeywordToken ClassKeyword(TextSpan span)
        {
            return new ClassKeywordToken(span);
        }
        public static IFunctionKeywordToken FunctionKeyword(TextSpan span)
        {
            return new FunctionKeywordToken(span);
        }
        public static INewKeywordToken NewKeyword(TextSpan span)
        {
            return new NewKeywordToken(span);
        }
        public static IOwnedKeywordToken OwnedKeyword(TextSpan span)
        {
            return new OwnedKeywordToken(span);
        }
        public static IForeverKeywordToken ForeverKeyword(TextSpan span)
        {
            return new ForeverKeywordToken(span);
        }
        public static INamespaceKeywordToken NamespaceKeyword(TextSpan span)
        {
            return new NamespaceKeywordToken(span);
        }
        public static IUsingKeywordToken UsingKeyword(TextSpan span)
        {
            return new UsingKeywordToken(span);
        }
        public static IForeachKeywordToken ForeachKeyword(TextSpan span)
        {
            return new ForeachKeywordToken(span);
        }
        public static IInKeywordToken InKeyword(TextSpan span)
        {
            return new InKeywordToken(span);
        }
        public static IIfKeywordToken IfKeyword(TextSpan span)
        {
            return new IfKeywordToken(span);
        }
        public static IElseKeywordToken ElseKeyword(TextSpan span)
        {
            return new ElseKeywordToken(span);
        }
        public static IUnsafeKeywordToken UnsafeKeyword(TextSpan span)
        {
            return new UnsafeKeywordToken(span);
        }
        public static ISafeKeywordToken SafeKeyword(TextSpan span)
        {
            return new SafeKeywordToken(span);
        }
        public static ISelfKeywordToken SelfKeyword(TextSpan span)
        {
            return new SelfKeywordToken(span);
        }
        public static IMutableKeywordToken MutableKeyword(TextSpan span)
        {
            return new MutableKeywordToken(span);
        }
        public static INoneKeywordToken NoneKeyword(TextSpan span)
        {
            return new NoneKeywordToken(span);
        }
        public static IMoveKeywordToken MoveKeyword(TextSpan span)
        {
            return new MoveKeywordToken(span);
        }
        public static ICopyKeywordToken CopyKeyword(TextSpan span)
        {
            return new CopyKeywordToken(span);
        }
        public static ILoopKeywordToken LoopKeyword(TextSpan span)
        {
            return new LoopKeywordToken(span);
        }
        public static IWhileKeywordToken WhileKeyword(TextSpan span)
        {
            return new WhileKeywordToken(span);
        }
        public static IBreakKeywordToken BreakKeyword(TextSpan span)
        {
            return new BreakKeywordToken(span);
        }
        public static INextKeywordToken NextKeyword(TextSpan span)
        {
            return new NextKeywordToken(span);
        }
        public static IAnyKeywordToken AnyKeyword(TextSpan span)
        {
            return new AnyKeywordToken(span);
        }
        public static ITrueKeywordToken TrueKeyword(TextSpan span)
        {
            return new TrueKeywordToken(span);
        }
        public static IFalseKeywordToken FalseKeyword(TextSpan span)
        {
            return new FalseKeywordToken(span);
        }
        public static IAsKeywordToken AsKeyword(TextSpan span)
        {
            return new AsKeywordToken(span);
        }
        public static IAndKeywordToken AndKeyword(TextSpan span)
        {
            return new AndKeywordToken(span);
        }
        public static IOrKeywordToken OrKeyword(TextSpan span)
        {
            return new OrKeywordToken(span);
        }
        public static INotKeywordToken NotKeyword(TextSpan span)
        {
            return new NotKeywordToken(span);
        }
    }

    [Closed(
        typeof(IPublishedKeywordToken),
        typeof(IPublicKeywordToken),
        typeof(ILetKeywordToken),
        typeof(IVarKeywordToken),
        typeof(IVoidKeywordToken),
        typeof(IIntKeywordToken),
        typeof(IUIntKeywordToken),
        typeof(IByteKeywordToken),
        typeof(ISizeKeywordToken),
        typeof(IOffsetKeywordToken),
        typeof(IBoolKeywordToken),
        typeof(INeverKeywordToken),
        typeof(IReturnKeywordToken),
        typeof(IClassKeywordToken),
        typeof(IFunctionKeywordToken),
        typeof(INewKeywordToken),
        typeof(IOwnedKeywordToken),
        typeof(IForeverKeywordToken),
        typeof(INamespaceKeywordToken),
        typeof(IUsingKeywordToken),
        typeof(IForeachKeywordToken),
        typeof(IInKeywordToken),
        typeof(IIfKeywordToken),
        typeof(IElseKeywordToken),
        typeof(IUnsafeKeywordToken),
        typeof(ISafeKeywordToken),
        typeof(ISelfKeywordToken),
        typeof(IMutableKeywordToken),
        typeof(INoneKeywordToken),
        typeof(IMoveKeywordToken),
        typeof(ICopyKeywordToken),
        typeof(ILoopKeywordToken),
        typeof(IWhileKeywordToken),
        typeof(IBreakKeywordToken),
        typeof(INextKeywordToken),
        typeof(IAnyKeywordToken),
        typeof(ITrueKeywordToken),
        typeof(IFalseKeywordToken),
        typeof(IAsKeywordToken),
        typeof(IAndKeywordToken),
        typeof(IOrKeywordToken),
        typeof(INotKeywordToken))]
    public partial interface IKeywordToken : IToken { }


    public partial interface IPublishedKeywordToken : IKeywordToken { }
    internal partial class PublishedKeywordToken : Token, IPublishedKeywordToken
    {
        public PublishedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPublicKeywordToken : IKeywordToken { }
    internal partial class PublicKeywordToken : Token, IPublicKeywordToken
    {
        public PublicKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILetKeywordToken : IKeywordToken { }
    internal partial class LetKeywordToken : Token, ILetKeywordToken
    {
        public LetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IVarKeywordToken : IKeywordToken { }
    internal partial class VarKeywordToken : Token, IVarKeywordToken
    {
        public VarKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IVoidKeywordToken : IKeywordToken { }
    internal partial class VoidKeywordToken : Token, IVoidKeywordToken
    {
        public VoidKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IIntKeywordToken : IKeywordToken { }
    internal partial class IntKeywordToken : Token, IIntKeywordToken
    {
        public IntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUIntKeywordToken : IKeywordToken { }
    internal partial class UIntKeywordToken : Token, IUIntKeywordToken
    {
        public UIntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IByteKeywordToken : IKeywordToken { }
    internal partial class ByteKeywordToken : Token, IByteKeywordToken
    {
        public ByteKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISizeKeywordToken : IKeywordToken { }
    internal partial class SizeKeywordToken : Token, ISizeKeywordToken
    {
        public SizeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOffsetKeywordToken : IKeywordToken { }
    internal partial class OffsetKeywordToken : Token, IOffsetKeywordToken
    {
        public OffsetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IBoolKeywordToken : IKeywordToken { }
    internal partial class BoolKeywordToken : Token, IBoolKeywordToken
    {
        public BoolKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INeverKeywordToken : IKeywordToken { }
    internal partial class NeverKeywordToken : Token, INeverKeywordToken
    {
        public NeverKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IReturnKeywordToken : IKeywordToken { }
    internal partial class ReturnKeywordToken : Token, IReturnKeywordToken
    {
        public ReturnKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IClassKeywordToken : IKeywordToken { }
    internal partial class ClassKeywordToken : Token, IClassKeywordToken
    {
        public ClassKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IFunctionKeywordToken : IKeywordToken { }
    internal partial class FunctionKeywordToken : Token, IFunctionKeywordToken
    {
        public FunctionKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INewKeywordToken : IKeywordToken { }
    internal partial class NewKeywordToken : Token, INewKeywordToken
    {
        public NewKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOwnedKeywordToken : IKeywordToken { }
    internal partial class OwnedKeywordToken : Token, IOwnedKeywordToken
    {
        public OwnedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IForeverKeywordToken : IKeywordToken { }
    internal partial class ForeverKeywordToken : Token, IForeverKeywordToken
    {
        public ForeverKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INamespaceKeywordToken : IKeywordToken { }
    internal partial class NamespaceKeywordToken : Token, INamespaceKeywordToken
    {
        public NamespaceKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUsingKeywordToken : IKeywordToken { }
    internal partial class UsingKeywordToken : Token, IUsingKeywordToken
    {
        public UsingKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IForeachKeywordToken : IKeywordToken { }
    internal partial class ForeachKeywordToken : Token, IForeachKeywordToken
    {
        public ForeachKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IInKeywordToken : IKeywordToken { }
    internal partial class InKeywordToken : Token, IInKeywordToken
    {
        public InKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IIfKeywordToken : IKeywordToken { }
    internal partial class IfKeywordToken : Token, IIfKeywordToken
    {
        public IfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IElseKeywordToken : IKeywordToken { }
    internal partial class ElseKeywordToken : Token, IElseKeywordToken
    {
        public ElseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUnsafeKeywordToken : IKeywordToken { }
    internal partial class UnsafeKeywordToken : Token, IUnsafeKeywordToken
    {
        public UnsafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISafeKeywordToken : IKeywordToken { }
    internal partial class SafeKeywordToken : Token, ISafeKeywordToken
    {
        public SafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISelfKeywordToken : IKeywordToken { }
    internal partial class SelfKeywordToken : Token, ISelfKeywordToken
    {
        public SelfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMutableKeywordToken : IKeywordToken { }
    internal partial class MutableKeywordToken : Token, IMutableKeywordToken
    {
        public MutableKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INoneKeywordToken : IKeywordToken { }
    internal partial class NoneKeywordToken : Token, INoneKeywordToken
    {
        public NoneKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMoveKeywordToken : IKeywordToken { }
    internal partial class MoveKeywordToken : Token, IMoveKeywordToken
    {
        public MoveKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICopyKeywordToken : IKeywordToken { }
    internal partial class CopyKeywordToken : Token, ICopyKeywordToken
    {
        public CopyKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILoopKeywordToken : IKeywordToken { }
    internal partial class LoopKeywordToken : Token, ILoopKeywordToken
    {
        public LoopKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IWhileKeywordToken : IKeywordToken { }
    internal partial class WhileKeywordToken : Token, IWhileKeywordToken
    {
        public WhileKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IBreakKeywordToken : IKeywordToken { }
    internal partial class BreakKeywordToken : Token, IBreakKeywordToken
    {
        public BreakKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INextKeywordToken : IKeywordToken { }
    internal partial class NextKeywordToken : Token, INextKeywordToken
    {
        public NextKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAnyKeywordToken : IKeywordToken { }
    internal partial class AnyKeywordToken : Token, IAnyKeywordToken
    {
        public AnyKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ITrueKeywordToken : IKeywordToken { }
    internal partial class TrueKeywordToken : Token, ITrueKeywordToken
    {
        public TrueKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IFalseKeywordToken : IKeywordToken { }
    internal partial class FalseKeywordToken : Token, IFalseKeywordToken
    {
        public FalseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsKeywordToken : IKeywordToken { }
    internal partial class AsKeywordToken : Token, IAsKeywordToken
    {
        public AsKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAndKeywordToken : IKeywordToken { }
    internal partial class AndKeywordToken : Token, IAndKeywordToken
    {
        public AndKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOrKeywordToken : IKeywordToken { }
    internal partial class OrKeywordToken : Token, IOrKeywordToken
    {
        public OrKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INotKeywordToken : IKeywordToken { }
    internal partial class NotKeywordToken : Token, INotKeywordToken
    {
        public NotKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
