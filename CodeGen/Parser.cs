using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.CodeGen.Config;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    internal static class Parser
    {
        public static Grammar ReadGrammarConfig(string grammar)
        {
            var lines = new List<string>();
            using (var reader = new StringReader(grammar))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
            }

            var ns = GetConfig(lines, "namespace");
            var baseType = ParseSymbol(GetConfig(lines, "base"));
            var prefix = GetConfig(lines, "prefix") ?? "";
            var suffix = GetConfig(lines, "suffix") ?? "";
            var listType = GetConfig(lines, "list") ?? "List";
            var usingNamespaces = GetUsingNamespaces(lines);
            var rules = GetRules(lines).Select(r => DefaultBaseType(r, baseType));
            return new Grammar(ns, baseType, prefix, suffix, listType, usingNamespaces, rules);
        }

        private static string? GetConfig(IEnumerable<string> lines, string config)
        {
            var start = Program.DirectiveMarker + config;
            var line = lines.SingleOrDefault(l => l.StartsWith(start, StringComparison.InvariantCulture));
            line = line?.Substring(start.Length);
            line = line?.TrimEnd(';'); // TODO error if no semicolon
            line = line?.Trim();
            return line;
        }

        private static IEnumerable<string> GetUsingNamespaces(IEnumerable<string> lines)
        {
            const string start = Program.DirectiveMarker + "using";
            lines = lines.Where(l => l.StartsWith(start, StringComparison.InvariantCulture));
            // TODO error if no semicolon
            return lines.Select(l => l.Substring(start.Length).TrimEnd(';').Trim());
        }

        private static GrammarRule DefaultBaseType(GrammarRule rule, GrammarSymbol? baseType)
        {
            if (baseType is null
                || rule.Parents.Any()
                || rule.Nonterminal == baseType) return rule;
            return new GrammarRule(rule.Nonterminal, baseType.YieldValue(), rule.Properties);
        }

        private static IEnumerable<GrammarRule> GetRules(List<string> lines)
        {
            var ruleLines = lines.Select(l => l.Trim())
                                 .Where(l => !l.StartsWith(Program.DirectiveMarker, StringComparison.InvariantCulture)
                                            && !l.StartsWith("//", StringComparison.InvariantCulture)
                                            && !string.IsNullOrWhiteSpace(l))
                                 .Select(l => l.TrimEnd(';')) // TODO error if no semicolon
                                 .ToList()!;
            foreach (var ruleLine in ruleLines)
            {
                var equalSplit = ruleLine.Split('=');
                if (equalSplit.Length > 2)
                    throw new FormatException($"Too many equal signs on line: '{ruleLine}'");
                var declaration = equalSplit[0];
                var (nonterminal, parents) = ParseDeclaration(declaration);
                var definition = equalSplit.Length == 2 ? equalSplit[1].Trim() : null;
                var properties = ParseDefinition(definition);
                yield return new GrammarRule(nonterminal, parents, properties);
            }
        }

        private static IEnumerable<GrammarProperty> ParseDefinition(string? definition)
        {
            if (definition is null) yield break;

            var properties = definition.SplitOrEmpty(' ').Where(v => !string.IsNullOrWhiteSpace(v));
            foreach (var property in properties)
            {
                var isList = property.EndsWith('*');
                var trimmedProperty = isList ? property[..^1] : property;
                var isOptional = property.EndsWith('?');
                trimmedProperty = isOptional ? trimmedProperty[..^1] : trimmedProperty;
                var parts = trimmedProperty.Split(':').Select(p => p.Trim()).ToArray();

                switch (parts.Length)
                {
                    case 1:
                    {
                        var name = parts[0];
                        yield return new GrammarProperty(name, new GrammarSymbol(name), isOptional, isList);
                    }
                    break;
                    case 2:
                    {
                        var name = parts[0];
                        var type = parts[1];
                        yield return new GrammarProperty(name, ParseSymbol(type), isOptional, isList);
                    }
                    break;
                    default:
                        throw new FormatException($"Too many colons in definition: '{definition}'");
                }
            }
        }

        private static (GrammarSymbol nonterminal, IEnumerable<GrammarSymbol> parents) ParseDeclaration(string declaration)
        {
            var declarationSplit = declaration.SplitOrEmpty(':');
            if (declarationSplit.Count > 2) throw new FormatException($"Too many colons in declaration: '{declaration}'");
            var nonterminal = ParseSymbol(declarationSplit[0].Trim());
            var parents = declarationSplit.Count == 2 ? declarationSplit[1] : null;
            var parentSymbols = ParseParents(parents);
            return (nonterminal, parentSymbols);
        }

        private static IEnumerable<GrammarSymbol> ParseParents(string? parents)
        {
            if (parents is null) return Enumerable.Empty<GrammarSymbol>();

            return parents
                   .Split(',')
                   .Select(p => p.Trim())
                   .Select(p => ParseSymbol(p));
        }

        [return: NotNullIfNotNull("symbol")]
        private static GrammarSymbol? ParseSymbol(string? symbol)
        {
            if (symbol is null) return null;
            if (symbol.StartsWith('\'') && symbol.EndsWith('\'')) return new GrammarSymbol(symbol[1..^1], true);
            return new GrammarSymbol(symbol);
        }
    }
}
