using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class PackageIL
    {
        public PackageSymbol Symbol { get; }
        public FixedSymbolTree SymbolTree { get; }
        public FixedList<Diagnostic> Diagnostics { get; internal set; }
        /// <summary>
        /// Referenced packages
        /// </summary>
        /// <remarks>No longer need aliases. All references are now explicit.</remarks>
        public FixedList<PackageIL> References { get; }
        public FixedList<DeclarationIL> Declarations { get; }
        public FunctionIL EntryPoint { get; internal set; }

        public PackageIL(
            FixedSymbolTree symbolTree,
            FixedList<Diagnostic> diagnostics,
            FixedList<PackageIL> references,
            IEnumerable<DeclarationIL> declarations,
            FunctionIL entryPoint)
        {
            Symbol = symbolTree.Package;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
            EntryPoint = entryPoint;
            Declarations = declarations.ToFixedList();
        }

        public IEnumerable<DeclarationIL> GetNonMemberDeclarations()
        {
            return Declarations.Where(d => !d.IsMember);
        }
    }
}
