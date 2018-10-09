using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public static partial class Keywords
    {
        [NotNull]
        public static readonly IReadOnlyList<Type> TokenTypes = new List<Type>()
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
        }.AsReadOnly();
    }

    public class PublicKeywordToken : KeywordToken
    {
        public PublicKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class ProtectedKeywordToken : KeywordToken
    {
        public ProtectedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class PrivateKeywordToken : KeywordToken
    {
        public PrivateKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class LetKeywordToken : KeywordToken
    {
        public LetKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class VarKeywordToken : KeywordToken
    {
        public VarKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class VoidKeywordToken : KeywordToken
    {
        public VoidKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class IntKeywordToken : KeywordToken
    {
        public IntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class UIntKeywordToken : KeywordToken
    {
        public UIntKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class ByteKeywordToken : KeywordToken
    {
        public ByteKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class SizeKeywordToken : KeywordToken
    {
        public SizeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class BoolKeywordToken : KeywordToken
    {
        public BoolKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class StringKeywordToken : KeywordToken
    {
        public StringKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class ReturnKeywordToken : KeywordToken
    {
        public ReturnKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class ClassKeywordToken : KeywordToken
    {
        public ClassKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class FunctionKeywordToken : KeywordToken
    {
        public FunctionKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class NewKeywordToken : KeywordToken
    {
        public NewKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class InitKeywordToken : KeywordToken
    {
        public InitKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class DeleteKeywordToken : KeywordToken
    {
        public DeleteKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class OwnedKeywordToken : KeywordToken
    {
        public OwnedKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class NamespaceKeywordToken : KeywordToken
    {
        public NamespaceKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class UsingKeywordToken : KeywordToken
    {
        public UsingKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class ForeachKeywordToken : KeywordToken
    {
        public ForeachKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class InKeywordToken : KeywordToken
    {
        public InKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class IfKeywordToken : KeywordToken
    {
        public IfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class ElseKeywordToken : KeywordToken
    {
        public ElseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class StructKeywordToken : KeywordToken
    {
        public StructKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class EnumKeywordToken : KeywordToken
    {
        public EnumKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class UnsafeKeywordToken : KeywordToken
    {
        public UnsafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class SafeKeywordToken : KeywordToken
    {
        public SafeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class SelfKeywordToken : KeywordToken
    {
        public SelfKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class SelfTypeKeywordToken : KeywordToken
    {
        public SelfTypeKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class BaseKeywordToken : KeywordToken
    {
        public BaseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
