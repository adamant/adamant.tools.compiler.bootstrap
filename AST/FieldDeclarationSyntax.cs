using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldDeclarationSyntax : MemberDeclarationSyntax, IBindingSymbol
    {
        public FixedList<IModiferToken> Modifiers { get; }
        public bool IsMutableBinding { get; }
        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax Initializer { get; }
        public TypePromise Type { get; } = new TypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType IBindingSymbol.Type => Type.Fulfilled();

        public FieldDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            bool mutableBinding,
            Name fullName,
            TextSpan nameSpan,
            ExpressionSyntax typeExpression,
            ExpressionSyntax initializer)
            : base(file, fullName, nameSpan)
        {
            Modifiers = modifiers;
            IsMutableBinding = mutableBinding;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        public override string ToString()
        {
            var result = $"{Name}: {TypeExpression}";
            if (Initializer != null)
                result += Initializer.ToString();
            result += ";";
            return result;
        }
    }
}
