using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAttributes
    {
        public PackageSyntax Package { get; }
        private readonly ReadOnlyDictionary<SyntaxBranchNode, ConcurrentDictionary<string, Lazy<object>>> values;

        public SemanticAttributes(PackageSyntax package)
        {
            Package = package;
            values = package.DescendantBranchesAndSelf()
                .ToDictionary(branch => branch,
                    branch => new ConcurrentDictionary<string, Lazy<object>>())
                .AsReadOnly();
        }

        public TReturn GetOrAdd<TSyntax, TReturn>(TSyntax syntax, string attribute, Func<TSyntax, TReturn> valueFactory)
            where TSyntax : SyntaxBranchNode
        {
            var lazy = values[syntax].GetOrAdd(attribute, (a, s) => new Lazy<object>(valueFactory(s)), syntax);
            return (TReturn)lazy.Value;
        }

        public TReturn GetOrAdd<TReturn>(SyntaxBranchNode syntax, string attribute, Lazy<object> value)
        {
            var lazy = values[syntax].GetOrAdd(attribute, value);
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
