using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    /// <summary>
    /// Builds up a hierarchy of lexical scope objects that are later used to look
    /// up names for name binding.
    ///
    /// Package qualified names are not supported yet, so currently, all names
    /// get placed into a single unified tree of namespace symbols.
    /// </summary>
    public class PackageLexicalScopesBuilder
    {
        private readonly Diagnostics diagnostics;
        private readonly FixedList<Namespace> namespaces;
        public GlobalScope GlobalScope { get; }

        public PackageLexicalScopesBuilder(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references,
            Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
            var namespaceNames = GetNamespaceNames(packageSyntax, references);
            var nonMemberEntitySymbols = GetNonMemberEntitySymbols(packageSyntax, references);
            namespaces = BuildNamespaces(namespaceNames, nonMemberEntitySymbols).ToFixedList();
            var allSymbols = nonMemberEntitySymbols.Concat(namespaces).ToFixedList();
            var globalSymbols = allSymbols.ToLookup(s => s.IsGlobal());
            GlobalScope = new GlobalScope(globalSymbols[true], globalSymbols[false]);
        }

        /// <summary>
        /// A distinct list of namespaces names in this or any referenced package.
        ///
        /// Referenced packages don't directly declare namespaces, they are implied
        /// by the names of the declared entities. Thus they can't contain empty namespaces.
        /// However, the syntax of a package has both namespace declarations and implicit
        /// namespaces of compilation units and can have empty namespaces.
        /// </summary>
        private static IEnumerable<Name> GetNamespaceNames(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references)
        {
            return
                // Any namespace created by primitive symbols
                PrimitiveSymbols.Instance.SelectMany(s => s.FullName.NestedInNames())
                // or any namespace containing a referenced entity
                .Concat(references.Values.SelectMany(p => p.GetNonMemberDeclarations())
                          .SelectMany(d => d.FullName.NestedInNames()))
                // or any namespace of a compilation unit
                .Concat(packageSyntax.CompilationUnits.SelectMany(cu => cu.ImplicitNamespaceName.NamespaceNames()))
                // or any declared namespace
                .Concat(packageSyntax.GetDeclarations().OfType<INamespaceDeclarationSyntax>().SelectMany(ns => ns.FullName.NamespaceNames()))
                .Distinct();
        }

        private static FixedList<ISymbol> GetNonMemberEntitySymbols(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references)
        {
            return references.Values.SelectMany(p => p.GetNonMemberDeclarations()).ImplicitCast<ISymbol>()
                             .Concat(packageSyntax.GetDeclarations().OfType<INonMemberEntityDeclarationSyntax>())
                             .Concat(PrimitiveSymbols.Instance).ToFixedList();
        }

        private static IEnumerable<Namespace> BuildNamespaces(IEnumerable<Name> namespaceNames, IEnumerable<ISymbol> nonMemberEntitySymbols)
        {
            var symbols = new List<ISymbol>(nonMemberEntitySymbols);
            // Process longest to shortest so nested namespaces will be available to construct outer namespaces
            foreach (var namespaceName in namespaceNames.OrderByDescending(n => n.Segments.Count()))
            {
                var nestedSymbols = symbols.Where(s => s.FullName.IsNestedIn(namespaceName))
                                           .ToLookup(s => s.FullName.HasQualifier(namespaceName));
                var ns = new Namespace(namespaceName, nestedSymbols[true], nestedSymbols[false]);
                yield return ns;
                symbols.Add(ns);
            }
        }

        public void BuildScopesFor(PackageSyntax package)
        {
            foreach (var compilationUnit in package.CompilationUnits)
            {
                var lexicalScopesWalker = new SyntaxLexicalScopesBuilder(compilationUnit.CodeFile, GlobalScope, namespaces, diagnostics);
                lexicalScopesWalker.Walk(compilationUnit, GlobalScope);
            }
        }
    }
}
