using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class Package
    {
        public PackageSymbol Symbol { get; }
        public FixedSymbolTree SymbolTree { get; }
        public FixedList<Diagnostic> Diagnostics { get; internal set; }
        /// <summary>
        /// Referenced packages
        /// </summary>
        /// <remarks>No longer need aliases. All references are now explicit.</remarks>
        public FixedList<Package> References { get; }
        public FixedList<Declaration> Declarations { get; }
        public FunctionDeclaration EntryPoint { get; internal set; }

        public Package(
            FixedSymbolTree symbolTree,
            FixedList<Diagnostic> diagnostics,
            FixedList<Package> references,
            IEnumerable<Declaration> declarations,
            FunctionDeclaration entryPoint)
        {
            Symbol = symbolTree.Package;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
            EntryPoint = entryPoint;
            Declarations = declarations.ToFixedList();
        }

        public IEnumerable<Declaration> GetNonMemberDeclarations()
        {
            return Declarations.Where(d => !d.IsMember);
        }
    }
}
