using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class Package
    {
        public FixedList<IDeclaration> AllDeclarations { get; }
        public FixedList<INonMemberDeclaration> NonMemberDeclarations { get; }
        public FixedSymbolTree SymbolTree { get; }
        public SymbolForest SymbolTrees { get; }
        public Diagnostics Diagnostics { get; }
        public FixedDictionary<Name, PackageIL> References { get; }
        public IEnumerable<PackageIL> ReferencedPackages => References.Values;

        public Package(
            FixedList<INonMemberDeclaration> nonMemberDeclarations,
            FixedSymbolTree symbolTree,
            Diagnostics diagnostics,
            FixedDictionary<Name, PackageIL> references)
        {
            AllDeclarations = GetAllDeclarations(nonMemberDeclarations).ToFixedList();
            NonMemberDeclarations = nonMemberDeclarations;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
            SymbolTrees = new SymbolForest(Primitive.SymbolTree, ReferencedPackages.Select(p => p.SymbolTree).Append(SymbolTree));
        }

        private static IEnumerable<IDeclaration> GetAllDeclarations(
            IEnumerable<INonMemberDeclaration> nonMemberDeclarations)
        {
            var declarations = new Queue<IDeclaration>();
            declarations.EnqueueRange(nonMemberDeclarations);
            while (declarations.TryDequeue(out var declaration))
            {
                yield return declaration;
                if (declaration is IClassDeclaration syn)
                    declarations.EnqueueRange(syn.Members);
            }
        }
    }
}
