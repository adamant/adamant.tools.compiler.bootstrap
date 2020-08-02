using System;
using System.Collections.Generic;
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
            var baseType = GetConfig(lines, "base");
            var prefix = GetConfig(lines, "prefix") ?? "";
            var suffix = GetConfig(lines, "suffix") ?? "";
            var listType = GetConfig(lines, "list") ?? "List";
            var usingNamespaces = GetUsingNamespaces(lines);
            var rules = GetRules(lines).ToList();
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

        private static IEnumerable<Rule> GetRules(List<string> lines)
        {
            var ruleLines = lines.Where(l => !l.StartsWith(Program.DirectiveMarker, StringComparison.InvariantCulture) && !string.IsNullOrWhiteSpace(l))
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
                yield return new Rule(nonterminal, parents, properties);
            }
        }

        private static (string nonterminal, List<string> parents) ParseDeclaration(string declaration)
        {
            var declarationSplit = declaration.Split(':');
            if (declarationSplit.Length > 2) throw new FormatException($"Too many colons in declaration: '{declaration}'");
            var nonterminal = declarationSplit[0].Trim();
            var parents = declarationSplit.Length == 2 ? declarationSplit[1] : null;
            var parentsSplit = parents?.Split(',').Select(p => p.Trim()).ToList() ?? new List<string>();
            return (nonterminal, parentsSplit);
        }

        private static IEnumerable<Property> ParseDefinition(string? definition)
        {
            if (definition is null) yield break;

            var properties = definition.SplitOrEmpty(' ').Where(v => !string.IsNullOrWhiteSpace(v));
            foreach (var property in properties)
            {
                var isList = property.EndsWith('*');
                var trimmedProperty = property.TrimEnd('*');
                var isOptional = property.EndsWith('?');
                trimmedProperty = trimmedProperty.TrimEnd('?');
                var parts = trimmedProperty.Split(':').Select(p => p.Trim()).ToArray();

                switch (parts.Length)
                {
                    case 1:
                    {
                        var name = parts[0];
                        yield return new Property(name, name, isOptional, isList);
                    }
                    break;
                    case 2:
                    {
                        var name = parts[0];
                        var type = parts[1];
                        yield return new Property(name, type, isOptional, isList);
                    }
                    break;
                    default:
                        throw new FormatException($"Too many colons in definition: '{definition}'");
                }
            }
        }
    }
}
