using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    internal static class Parser
    {
        public static GrammarConfig ReadGrammarConfig(string inputPath)
        {
            var lines = File.ReadLines(inputPath)?.ToList()
                        ?? throw new InvalidOperationException("null from reading input file");
            var ns = GetConfig(lines, "namespace");
            var baseClass = GetConfig(lines, "base");
            var suffix = GetConfig(lines, "suffix") ?? "";
            var listClass = GetConfig(lines, "list") ?? "List";
            var rules = GetRules(lines).ToList<RuleConfig>();
            return new GrammarConfig();
        }

        private static string? GetConfig(IEnumerable<string> lines, string config)
        {
            var start = Program.DirectiveMarker + config;
            var line = lines.SingleOrDefault(l => l.StartsWith(start));
            line = line?.Substring(start.Length);
            line = line?.TrimEnd(';'); // TODO error if no semicolon
            line = line?.Trim();
            return line;
        }

        private static IEnumerable<RuleConfig> GetRules(List<string> lines)
        {
            var ruleLines = lines.Where(l => !l.StartsWith(Program.DirectiveMarker) && !String.IsNullOrWhiteSpace(l))
                                 .Select(l => l.TrimEnd(';')) // TODO error if no semicolon
                                 .ToList()!;
            foreach (var ruleLine in ruleLines)
            {
                var equalSplit = ruleLine.Split('=');
                if (equalSplit.Length > 2)
                    throw new FormatException($"Too many equal signs on line: '{ruleLine}'");
                var declaration = equalSplit[0];
                var definition = equalSplit.Length == 2 ? equalSplit[1] : null;
                var declarationSplit = declaration.Split(':');
                if (declarationSplit.Length > 2)
                    throw new FormatException($"Too many colons in declaration: '{declaration}'");
                var nonterminal = declarationSplit[0].Trim();
                var parents = declarationSplit.Length == 2 ? declarationSplit[1] : null;
                var parentsSplit = parents?.Split(',').Select(p => p.Trim()).ToList();
                yield return new RuleConfig();
            }
        }
    }
}
