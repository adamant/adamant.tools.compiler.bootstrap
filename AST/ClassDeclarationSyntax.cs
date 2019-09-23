using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ClassDeclarationSyntax : TypeDeclarationSyntax
    {
        public FixedList<IModiferToken> Modifiers { get; }

        public ClassDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<MemberDeclarationSyntax> members)
            : base(span, file, nameSpan, fullName, members)
        {
            Modifiers = modifiers;
        }

        public void CreateDefaultConstructor()
        {
            if (Members.Any(m => m is ConstructorDeclarationSyntax))
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
