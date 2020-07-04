using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public static class ReachabilityGraphGraphvizExtensions
    {
        /// <summary>
        /// Convert the graph into DOT format supported by graphviz
        /// </summary>
        public static string ToDotFormat(this ReachabilityGraph graph)
        {
            var dot = new StringBuilder();
            var nodes = new Dictionary<MemoryPlace, string>();
            dot.AppendLine("digraph reachability {");
            dot.AppendLine("    rankdir=LR;");
            AppendStack(graph, dot, nodes);
            //AppendContextObjects(graph, dot, nodes);
            AppendObjects(graph, dot, nodes);
            AppendReferences(graph, dot, nodes);
            dot.AppendLine("}");
            return dot.ToString();
        }

        private static void AppendStack(ReachabilityGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            var hasCallerVariables = graph.CallerVariables.Any();
            var hasVariables = graph.Variables.Any();
            var hasTempValues = graph.TempValues.Any();
            if (!hasCallerVariables && !hasVariables && !hasTempValues) return;

            dot.AppendLine("    node [shape=plaintext];");
            dot.AppendLine("    stack [label=<");
            dot.AppendLine("        <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" CELLPADDING=\"4\">");
            var nextNode = 1;
            foreach (var callerVariable in graph.CallerVariables) AppendStackItem(dot, callerVariable, "cv", nodes, ref nextNode);

            if (hasCallerVariables && hasVariables) AppendStackSeparator(dot);

            nextNode = 1;
            foreach (var variable in graph.Variables) AppendStackItem(dot, variable, "v", nodes, ref nextNode);

            if ((hasCallerVariables || hasVariables) && hasTempValues) AppendStackSeparator(dot);

            nextNode = 1;
            foreach (var tempValue in graph.TempValues) AppendStackItem(dot, tempValue, "t", nodes, ref nextNode);

            dot.AppendLine("        </TABLE>>];"); // end stack
        }

        private static void AppendStackItem(
            StringBuilder dot,
            StackPlace place,
            string nodeType,
            Dictionary<MemoryPlace, string> nodes,
            ref int nextNode)
        {
            var port = nodeType + nextNode;
            nodes.Add(place, "stack:" + port + ":e");
            var label = Escape(place.ToString()!);
            var color = "";
            if (!place.IsAllocated)
            {
                color = " COLOR=\"red\"";
                label = $"<S>{label}</S>";
            }
            dot.AppendLine($"            <TR><TD PORT=\"{port}\" ALIGN=\"LEFT\"{color}>{label}</TD></TR>");
            nextNode += 1;
        }

        private static void AppendStackSeparator(StringBuilder dot)
        {
            dot.AppendLine("            <TR><TD CELLPADDING=\"0\" BGCOLOR=\"black\"></TD></TR>");
        }

        private static void AppendObjects(ReachabilityGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            if (!graph.Objects.Any()) return;

            dot.AppendLine("    node [shape=ellipse, peripheries=2];");

            var nextObject = 1;
            foreach (var contextObject in graph.Objects.Where(o => o.IsContext))
            {
                AppendObject(dot, contextObject, "ctx", nodes, nextObject);
                nextObject += 1;
            };

            dot.AppendLine("    node [shape=ellipse, peripheries=1];");

            nextObject = 1;
            foreach (var obj in graph.Objects.Where(o => !o.IsContext))
            {
                AppendObject(dot, obj, "obj", nodes, nextObject);
                nextObject += 1;
            }
        }

        private static void AppendObject(
            StringBuilder dot,
            Object obj,
            string objectType,
            Dictionary<MemoryPlace, string> nodes,
            int id)
        {
            var name = objectType + id;
            nodes.Add(obj, name);
            var label = Escape(obj.ToString());
            var color = "";
            if (!obj.IsAllocated)
            {
                color = ",color=Red";
                label = $"<S>{label}</S>";
            }
            dot.AppendLine($"    {name} [label=\"{label}\"{color}];");
        }

        private static void AppendReferences(ReachabilityGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            bool firstEdge = true;

            var pairs = graph.CallerVariables
                               .Concat<MemoryPlace>(graph.Variables)
                               .Concat(graph.TempValues)
                               .Concat(graph.Objects)
                               .SelectMany(source => source.References.Select(reference => (source, reference)))
                               .ToList();
            var referenceNames = new Dictionary<Reference, string>();
            foreach (var (sourceNode, reference) in pairs)
            {
                if (firstEdge)
                {
                    dot.AppendLine("    node [shape=circle,width=0.05,label=\"\"];");
                    dot.AppendLine("    edge [dir=both];");
                    firstEdge = false;
                }

                AppendReference(dot, sourceNode, reference, nodes, referenceNames);
            }

            var borrowedReferences = pairs.Select(p => p.reference).Where(r => r.Borrowers.Any());
            firstEdge = true;
            foreach (var borrowed in borrowedReferences)
            {
                if (firstEdge)
                {
                    dot.AppendLine("    edge [dir=forward,arrowhead=normal,style=dashed];");
                }

                var borrowedEdgeName = referenceNames[borrowed];
                foreach (var borrower in borrowed.Borrowers)
                {
                    var color = ColorForPhase(borrower);
                    dot.AppendLine($"    {borrowedEdgeName} -> {referenceNames[borrower]} [color=\"{color}:{color}\"];");
                }
            }
        }

        private static void AppendReference(
            StringBuilder dot,
            MemoryPlace sourceNode,
            Reference reference,
            Dictionary<MemoryPlace, string> nodes,
            Dictionary<Reference, string> referenceNames)
        {
            var involvedInBorrowing = reference.IsUsedForBorrow() || (reference.DeclaredAccess == Access.Mutable && !reference.CouldHaveOwnership);

            if (involvedInBorrowing)
            {
                // Declare a "middle" node
                var name = "r" + (referenceNames.Count + 1);
                referenceNames.Add(reference, name);
                dot.Append($"    {name} [");
                AppendDeclaredAccess(dot, reference);
                dot.AppendLine("];");

                // Start to Middle
                dot.Append($"    {nodes[sourceNode]} -> {name} [dir=back,");
                AppendOwnership(dot, reference);
                dot.Append(",");
                AppendDeclaredAccess(dot, reference);
                dot.AppendLine("];");

                // Middle to End
                dot.Append($"    {name} -> {nodes[reference.Referent]} [dir=forward,");
                AppendDeclaredAccess(dot, reference);
                dot.Append(",");
                AppendEffectiveAccess(dot, reference);
                dot.AppendLine("];");
            }
            else
            {
                dot.Append($"    {nodes[sourceNode]} -> {nodes[reference.Referent]} [");
                AppendOwnership(dot, reference);
                dot.Append(",");
                AppendDeclaredAccess(dot, reference);
                dot.Append(",");
                AppendEffectiveAccess(dot, reference);
                dot.AppendLine("];");
            }
        }

        private static void AppendOwnership(StringBuilder dot, Reference reference)
        {
            switch (reference.Ownership)
            {
                default:
                    throw ExhaustiveMatch.Failed(reference.Ownership);
                case Ownership.None:
                    dot.Append("arrowtail=odiamond");
                    break;
                case Ownership.Owns:
                    dot.Append("arrowtail=diamond");
                    break;
                case Ownership.PotentiallyOwns:
                    dot.Append("arrowtail=normaloinv");
                    break;
            }
        }

        private static void AppendDeclaredAccess(StringBuilder dot, Reference reference)
        {
            var color = ColorForPhase(reference);

            switch (reference.DeclaredAccess)
            {
                default:
                    throw ExhaustiveMatch.Failed(reference.DeclaredAccess);
                case Access.Identify:
                    dot.Append($"color={color},style=dashed");
                    break;
                case Access.Mutable:
                    dot.Append($"color=\"{color}:{color}\"");
                    break;
                case Access.ReadOnly:
                    dot.Append("color=" + color);
                    break;
            }
        }

        private static string ColorForPhase(Reference reference)
        {
            string color;
            switch (reference.Phase)
            {
                default:
                    throw ExhaustiveMatch.Failed(reference.Phase);
                case Phase.Unused:
                    color = "grey40";
                    break;
                case Phase.Used:
                    color = "black";
                    break;
                case Phase.Released:
                    color = "red";
                    break;
            }

            return color;
        }

        private static void AppendEffectiveAccess(StringBuilder dot, Reference reference)
        {
            switch (reference.EffectiveAccess())
            {
                default:
                    throw ExhaustiveMatch.Failed(reference.DeclaredAccess);
                case Access.Identify:
                    dot.Append("arrowhead=onormalicurve");
                    break;
                case Access.Mutable:
                    dot.Append("arrowhead=normal");
                    break;
                case Access.ReadOnly:
                    dot.Append("arrowhead=onormal");
                    break;
            }
        }

        private static string Escape(string label)
        {
            return label.Replace("⟦", "&laquo;", StringComparison.InvariantCulture)
                        .Replace("⟧", "&raquo;", StringComparison.InvariantCulture)
                        .Replace("\\", "\\\\", StringComparison.Ordinal)
                        .Replace("\"", "\\\"", StringComparison.Ordinal);
        }
    }
}
