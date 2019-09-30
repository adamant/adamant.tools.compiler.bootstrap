using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ClassDeclarationSyntax : DeclarationSyntax, IClassDeclarationSyntax
    {
        public FixedList<IModiferToken> Modifiers { get; }
        public Name FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public FixedList<MemberDeclarationSyntax> Members { get; }
        public TypePromise DeclaresType { get; } = new TypePromise();
        public SymbolSet ChildSymbols { get; protected set; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ITypeSymbol.DeclaresType => DeclaresType.Fulfilled();

        public ClassDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<MemberDeclarationSyntax> members)
            : base(span, file, nameSpan)
        {
            Modifiers = modifiers;
            FullName = fullName;
            Members = members;
            foreach (var member in Members)
                member.DeclaringType = this;
            ChildSymbols = new SymbolSet(members);
        }

        public void CreateDefaultConstructor()
        {
            if (Members.Any(m => m is IConstructorDeclarationSyntax))
                return;

            var constructor = new DefaultConstructor((UserObjectType)DeclaresType.Fulfilled());
            ChildSymbols = new SymbolSet(ChildSymbols.Values.SelectMany(s => s).Append(constructor));
        }

        public override string ToString()
        {
            return $"class {FullName} {{ … }}";
        }
    }
}
