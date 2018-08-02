using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class SemanticAttributes
    {
        public readonly PackageSyntax Package;

        public readonly TypeAttribute Type;
        public readonly ParentAttribute Parent;
        public readonly NameAttribute Name;
        public readonly LexicalScopeAttribute NameScope;
        public readonly SyntaxSymbolAttribute SyntaxSymbol;
        public readonly NodeAttribute Node;
        public readonly DiagnosticsAttribute Diagnostics;
        public readonly AllDiagnosticsAttribute AllDiagnostics;

        private readonly ReadOnlyDictionary<SyntaxBranchNode, ConcurrentDictionary<string, Lazy<object>>> values;

        public SemanticAttributes(PackageSyntax package)
        {
            Package = package;

            Type = new TypeAttribute(this);
            Parent = new ParentAttribute(this);
            Name = new NameAttribute(this);
            NameScope = new LexicalScopeAttribute(this);
            SyntaxSymbol = new SyntaxSymbolAttribute(this);
            Node = new NodeAttribute(this);
            Diagnostics = new DiagnosticsAttribute(this);
            AllDiagnostics = new AllDiagnosticsAttribute(this);

            values = AllBranchesWithParents(package).AsReadOnly();
        }

        private Dictionary<SyntaxBranchNode, ConcurrentDictionary<string, Lazy<object>>> AllBranchesWithParents(PackageSyntax package)
        {
            var attributes = new Dictionary<SyntaxBranchNode, ConcurrentDictionary<string, Lazy<object>>>();
            AllBranchesWithParents(attributes, package, null);
            return attributes;
        }

        private void AllBranchesWithParents(
            IDictionary<SyntaxBranchNode, ConcurrentDictionary<string, Lazy<object>>> attributes,
            SyntaxBranchNode syntax,
            SyntaxBranchNode parentSyntax)
        {
            var nodeAttributes = new ConcurrentDictionary<string, Lazy<object>>();
            var lazyParent = new Lazy<object>(parentSyntax);
            Debug.Assert(nodeAttributes.TryAdd(Parent.AttributeKey, lazyParent));
            attributes.Add(syntax, nodeAttributes);
            foreach (var child in syntax.Children.OfType<SyntaxBranchNode>())
                AllBranchesWithParents(attributes, child, syntax);
        }

        public TReturn GetOrAdd<TSyntax, TReturn>(TSyntax syntax, string attribute, Func<TSyntax, TReturn> factory)
            where TSyntax : SyntaxBranchNode
        {
            var lazy = values[syntax].GetOrAdd(attribute, (a, s) => new Lazy<object>(factory(s)), syntax);
            return (TReturn)lazy.Value;
        }

        public TReturn Get<TReturn>(SyntaxBranchNode syntax, string attribute)
        {
            if (values[syntax].TryGetValue(attribute, out var lazy))
                return (TReturn)lazy.Value;

            throw new Exception($"No '{attribute}' for node of type {syntax.GetType().Name}");
        }

        public bool HasAttribute(SyntaxBranchNode syntax, string attribute)
        {
            return values[syntax].ContainsKey(attribute);
        }
    }
}
