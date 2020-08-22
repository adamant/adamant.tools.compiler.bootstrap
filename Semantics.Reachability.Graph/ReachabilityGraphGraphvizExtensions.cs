using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public static class ReachabilityGraphGraphvizExtensions
    {
        private const string Grey = "grey55";
        private const string Black = "black";

        /// <summary>
        /// Convert the graph into DOT format supported by graphviz
        /// </summary>
        public static string ToDotFormat(this ReachabilityGraph graph)
        {
            return graph.ReferenceGraph.ToDotFormat();
        }

        internal static string ToDotFormat(this IReferenceGraph graph)
        {
            var dot = new StringBuilder();
            var nodes = new Dictionary<MemoryPlace, string>();
            dot.AppendLine("digraph reachability {");
            dot.AppendLine("    rankdir=LR;");
            AppendStack(graph, dot, nodes);
            AppendObjects(graph, dot, nodes);
            AppendReferences(graph, dot, nodes);
            dot.AppendLine("}");
            return dot.ToString();
        }

        private static void AppendStack(IReferenceGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
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

        private static void AppendObjects(IReferenceGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            if (!graph.Objects.Any()) return;

            dot.AppendLine("    node [shape=ellipse,style=diagonals];");

            var nextObject = 1;
            foreach (var contextObject in graph.Objects.Where(o => o.IsContext))
            {
                AppendObject(dot, contextObject, "ctx", nodes, nextObject);
                nextObject += 1;
            };

            dot.AppendLine("    node [shape=ellipse,style=solid];");

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
                label = $"<<S>{label}</S>>";
            }
            else
                label = $"\"{label}\"";

            var access = obj.GetCurrentAccess();
            var style = "";
            switch (access)
            {
                default:
                    throw ExhaustiveMatch.Failed(access);
                case Access.ReadOnly:
                    // solid is the default
                    break;
                case Access.Mutable:
                    style = ",peripheries=2";
                    break;
                case Access.Identify:
                    style = ",style=dashed";
                    break;
                case null:
                    style = ",style=dotted";
                    break;
            }
            dot.AppendLine($"    {name} [label={label}{color}{style}];");
        }

        private static void AppendReferences(IReferenceGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            var places = graph.CallerVariables
                              .Concat<MemoryPlace>(graph.Variables)
                              .Concat(graph.TempValues)
                              .Concat(graph.Objects)
                              .ToList();

            var referenceNames = new Dictionary<IReference, string>();

            AppendBorrows(dot, places, referenceNames);
            AppendReferences(dot, nodes, places, referenceNames);
        }

        private static void AppendBorrows(
            StringBuilder dot,
            IReadOnlyList<MemoryPlace> places,
            Dictionary<IReference, string> referenceNames)
        {
            var borrowedReferences = places.SelectMany(n => n.References)
                                           .Where(r => r.Borrowers.Any()).ToList();

            if (borrowedReferences.Any())
            {
                dot.AppendLine("    node [shape=circle,width=0.05,label=\"\"];");
                dot.AppendLine("    edge [dir=forward,style=dashed];");
            }

            foreach (var borrowed in borrowedReferences)
            {
                var borrowedName = AppendBorrowPoint(dot, borrowed, referenceNames);
                foreach (var borrower in borrowed.Borrowers)
                {
                    var borrowerName = AppendBorrowPoint(dot, borrower, referenceNames);
                    var color = borrower.IsUsed || borrower.IsUsedForBorrow() ? Black : Grey;
                    dot.AppendLine($"    {borrowedName} -> {borrowerName} [color=\"{color}:{color}\"];");
                }
            }
        }

        private static string AppendBorrowPoint(
            StringBuilder dot,
            IReference reference,
            Dictionary<IReference, string> referenceNames)
        {
            if (referenceNames.TryGetValue(reference, out var name)) return name;

            name = "r" + (referenceNames.Count + 1);
            referenceNames.Add(reference, name);
            var color = ColorForPhase(reference);
            dot.AppendLine($"    {name} [color={color}];");
            return name;
        }

        private static void AppendReferences(
            StringBuilder dot,
            Dictionary<MemoryPlace, string> nodes,
            List<MemoryPlace> places,
            Dictionary<IReference, string> referenceNames)
        {
            bool firstEdge = true;
            foreach (var sourceNode in places)
                foreach (var reference in sourceNode.References)
                {
                    if (firstEdge)
                    {
                        dot.AppendLine("    edge [dir=both,style=solid];");
                        firstEdge = false;
                    }

                    AppendReference(dot, sourceNode, reference, nodes, referenceNames);
                }
        }

        private static void AppendReference(
            StringBuilder dot,
            MemoryPlace sourceNode,
            IReference reference,
            Dictionary<MemoryPlace, string> nodes,
            Dictionary<IReference, string> referenceNames)
        {
            if (referenceNames.TryGetValue(reference, out var borrowPointName))
            {
                // Start to Middle
                dot.Append($"    {nodes[sourceNode]} -> {borrowPointName} [dir=back,");
                AppendOwnership(dot, reference);
                dot.Append(",");
                AppendDeclaredAccess(dot, reference);
                dot.AppendLine("];");

                // Middle to End
                dot.Append($"    {borrowPointName} -> {nodes[reference.Referent]} [dir=forward,");
                if (reference.IsUsedForBorrow())
                {
                    var color = ColorForPhase(reference);
                    dot.Append($"color={color},style=dashed");
                }
                else
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

        private static void AppendOwnership(StringBuilder dot, IReference reference)
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

        private static void AppendDeclaredAccess(StringBuilder dot, IReference reference)
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

        private static string ColorForPhase(IReference reference)
        {
            string color = reference.Phase switch
            {
                Phase.Unused => Grey,
                Phase.Used => Black,
                Phase.Released => "red",
                _ => throw ExhaustiveMatch.Failed(reference.Phase)
            };

            return color;
        }

        private static void AppendEffectiveAccess(StringBuilder dot, IReference reference)
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
