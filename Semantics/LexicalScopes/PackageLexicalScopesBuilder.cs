using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    /// <summary>
    /// Builds up a hierarchy of lexical scope objects that are later used to look
    /// up names for name binding
    /// </summary>
    public class PackageLexicalScopesBuilder
    {
        // TODO we need a list of all the namespaces for validating using statements?
        // Gather a list of all the namespaces for validating using statements
        // Also need to account for empty directories?

        private readonly Diagnostics diagnostics;
        private readonly FixedList<ISymbol> nonMemberEntitySymbols;
        private readonly GlobalScope globalScope;

        public PackageLexicalScopesBuilder(
             Diagnostics diagnostics,
             PackageSyntax packageSyntax,
             FixedDictionary<string, Package> references)
        {
            this.diagnostics = diagnostics;
            nonMemberEntitySymbols = GetNonMemberEntitySymbols(packageSyntax, references);
            globalScope = new GlobalScope(GetGlobalSymbols(nonMemberEntitySymbols), nonMemberEntitySymbols);
        }

        private static FixedList<ISymbol> GetNonMemberEntitySymbols(
             PackageSyntax packageSyntax,
             FixedDictionary<string, Package> references)
        {
            return references.Values
                .SelectMany(p => p.Declarations.Where(d => !d.IsMember))
                .Concat(packageSyntax.CompilationUnits.SelectMany(GetNonMemberEntitySymbols))
                .ToFixedList();
        }

        /// <summary>
        /// This gets the symbols for all entities that are declared outside of a class.
        /// (i.e. directly in a namespace)
        /// </summary>
        private static FixedList<ISymbol> GetNonMemberEntitySymbols(ICompilationUnitSyntax compilationUnit)
        {
            var declarations = new List<ISymbol>();
            declarations.AddRange(compilationUnit.Declarations.OfType<INonMemberEntityDeclarationSyntax>());
            var namespaces = new Queue<INamespaceDeclarationSyntax>();
            namespaces.EnqueueRange(compilationUnit.Declarations.OfType<INamespaceDeclarationSyntax>());
            while (namespaces.TryDequeue(out var ns))
            {
                declarations.AddRange(ns.Declarations.OfType<INonMemberEntityDeclarationSyntax>());
                namespaces.EnqueueRange(ns.Declarations.OfType<INamespaceDeclarationSyntax>());
            }

            return declarations.ToFixedList();
        }

        private static IEnumerable<ISymbol> GetGlobalSymbols(FixedList<ISymbol> symbols)
        {
            return symbols.Where(s => s.IsGlobal())
                .Concat(PrimitiveSymbols.Instance);
        }

        public void BuildScopesFor(PackageSyntax package)
        {
            var lexicalScopesWalker = new SyntaxLexicalScopesBuilder(nonMemberEntitySymbols, globalScope);
            foreach (var compilationUnit in package.CompilationUnits)
                lexicalScopesWalker.Walk(compilationUnit, globalScope);
        }
    }
}
