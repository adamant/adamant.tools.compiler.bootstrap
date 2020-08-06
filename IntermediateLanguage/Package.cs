using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class Package
    {
        public PackageSymbol Symbol { get; }
        public FixedList<Diagnostic> Diagnostics { get; internal set; }
        public FixedDictionary<Name, Package> References { get; }
        public FixedList<Declaration> Declarations { get; }
        public FunctionDeclaration EntryPoint { get; internal set; }

        public Package(
            PackageSymbol symbol,
            FixedList<Diagnostic> diagnostics,
            FixedDictionary<Name, Package> references,
            IEnumerable<Declaration> declarations,
            FunctionDeclaration entryPoint)
        {
            Symbol = symbol;
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
