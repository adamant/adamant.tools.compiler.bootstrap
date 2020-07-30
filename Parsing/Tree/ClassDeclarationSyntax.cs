using System;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ClassDeclarationSyntax : DeclarationSyntax, IClassDeclarationSyntax
    {
        public IAccessModifierToken? AccessModifier { get; }
        public IMutableKeywordToken? MutableModifier { get; }
        public Name FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public FixedList<IMemberDeclarationSyntax> Members { get; }
        public TypePromise DeclaresType { get; } = new TypePromise();
        public SymbolSet ChildSymbols { get; protected set; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ITypeSymbol.DeclaresType => DeclaresType.Fulfilled();

        public ClassDeclarationSyntax(
            TextSpan headerSpan,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            IMutableKeywordToken? mutableModifier,
            Name fullName,
            TextSpan nameSpan,
            Func<IClassDeclarationSyntax, (FixedList<IMemberDeclarationSyntax>, TextSpan)> parseMembers)
            : base(headerSpan, file, nameSpan)
        {
            AccessModifier = accessModifier;
            MutableModifier = mutableModifier;
            FullName = fullName;
            var (members, bodySpan) = parseMembers(this);
            Members = members;
            ChildSymbols = new SymbolSet(Members);
            Span = TextSpan.Covering(headerSpan, bodySpan);
        }

        public void CreateDefaultConstructor()
        {
            if (Members.Any(m => m is IConstructorDeclarationSyntax))
                return;

            var constructor = new DefaultConstructor((ObjectType)DeclaresType.Fulfilled());
            ChildSymbols = new SymbolSet(ChildSymbols.Append<ISymbol>(constructor));
        }

        public override string ToString()
        {
            return $"class {FullName} {{ … }}";
        }
    }
}
