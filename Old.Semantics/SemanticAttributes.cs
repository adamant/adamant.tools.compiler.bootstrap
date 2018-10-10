using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
{
    [NotNull]
    public delegate TReturn ValueFactory<in TSyntax, out TReturn>([NotNull] TSyntax syntax)
        where TSyntax : SyntaxNode;

    public class SemanticAttributes
    {
        [NotNull] public PackageSyntax Package { get; }
        [NotNull] private readonly ReadOnlyDictionary<SyntaxNode, ConcurrentDictionary<string, Lazy<object>>> values;

        public SemanticAttributes([NotNull] PackageSyntax package)
        {
            Requires.NotNull(nameof(package), package);
            Package = package;
            values = package.DescendantsAndSelf()
                .ToDictionary(branch => branch,
                    branch => new ConcurrentDictionary<string, Lazy<object>>())
                .AsReadOnly();
        }

        public TReturn GetOrAdd<TSyntax, TReturn>([NotNull] TSyntax syntax, [NotNull] string attribute, [NotNull] ValueFactory<TSyntax, TReturn> valueFactory)
            where TSyntax : SyntaxNode
        {
            var lazy = values[syntax].GetOrAdd(attribute, (a, s) => new Lazy<object>(valueFactory(s)), syntax);
            return (TReturn)lazy.Value;
        }

        public TReturn GetOrAdd<TReturn>([NotNull] SyntaxNode syntax, [NotNull] string attribute, [NotNull] Lazy<object> value)
        {
            var lazy = values[syntax].GetOrAdd(attribute, value);
            return (TReturn)lazy.Value;
        }

        public TReturn Get<TReturn>([NotNull] SyntaxNode syntax, [NotNull] string attribute)
        {
            if (values[syntax].TryGetValue(attribute, out var lazy))
                return (TReturn)lazy.Value;

            throw new Exception($"No '{attribute}' for node of type {syntax.GetType().Name}");
        }

        public bool HasAttribute([NotNull] SyntaxNode syntax, [NotNull] string attribute)
        {
            return values[syntax].ContainsKey(attribute);
        }
    }
}
