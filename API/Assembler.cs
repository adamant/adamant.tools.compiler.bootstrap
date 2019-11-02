using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class Assembler
    {
        protected const int StandardStatementWidth = 50;

        public string Disassemble(Package package)
        {
            var builder = new AssemblyBuilder();
            var typeMembers = package.Declarations.OfType<ClassDeclaration>()
                .SelectMany(t => t.Members).ToList();
            foreach (var declaration in package.Declarations.Except(typeMembers))
            {
                Disassemble(declaration, builder);
                builder.BlankLine();
            }

            return builder.Code;
        }

        public void Disassemble(Declaration declaration, AssemblyBuilder builder)
        {
            switch (declaration)
            {
                case FunctionDeclaration function:
                    Disassemble(function, builder);
                    break;
                case ConstructorDeclaration constructor:
                    Disassemble(constructor, builder);
                    break;
                case ClassDeclaration type:
                    Disassemble(type, builder);
                    break;
                case FieldDeclaration field:
                    Disassemble(field, builder);
                    break;
                default:
                    throw ExhaustiveMatch.Failed(declaration);
            }
        }

        private static void Disassemble(FunctionDeclaration function, AssemblyBuilder builder)
        {
            var parameters = FormatParameters(function.Parameters);
            builder.BeginLine("fn ");
            builder.Append(function.FullName.ToString());
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine(builder.IndentCharacters);
            builder.Append("-> ");
            builder.EndLine(function.ReturnType.ToString());
            if (function.ControlFlow != null)
            {
                builder.BeginBlock();
                Disassemble(function.ControlFlow, builder);
                builder.EndBlock();
            }
        }

        private static string FormatParameters(IEnumerable<Parameter> parameters)
        {
            var formatted = string.Join(", ", parameters.Select(FormatParameter));
            return formatted;
        }

        private static string FormatParameter(Parameter parameter)
        {
            var format = parameter.IsMutableBinding ? "var {0}: {1}" : "{0}: {1}";
            return string.Format(CultureInfo.InvariantCulture, format, parameter.Name, parameter.Type);
        }

        private static void Disassemble(ConstructorDeclaration constructor, AssemblyBuilder builder)
        {
            var parameters = FormatParameters(constructor.Parameters);
            builder.BeginLine("fn ");
            builder.Append(constructor.Name.ToString());
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine(builder.IndentCharacters);
            builder.Append("-> ");
            builder.EndLine(constructor.ReturnType.ToString());
            builder.BeginBlock();
            Disassemble(constructor.ControlFlow, builder);
            builder.EndBlock();
        }

        private void Disassemble(ClassDeclaration @class, AssemblyBuilder builder)
        {
            builder.BeginLine("type ");
            builder.Append(@class.FullName.ToString());
            builder.EndLine();
            builder.BeginBlock();
            foreach (var member in @class.Members)
            {
                builder.DeclarationSeparatorLine();
                Disassemble(member, builder);
            }
            builder.EndBlock();
        }

        private static void Disassemble(ControlFlowGraph graph, AssemblyBuilder builder)
        {
            if (graph == null)
            {
                builder.AppendLine("// Control Flow Graph not available");
                return;
            }

            foreach (var declaration in graph.LocalVariables)
                Disassemble(declaration, builder);

            if (graph.LocalVariables.Any(v => v.TypeIsNotEmpty))
                builder.BlankLine();

            if (Disassemble(graph.BorrowClaims?.ParameterClaims, builder))
                builder.BlankLine();

            foreach (var block in graph.BasicBlocks)
                Disassemble(graph, block, builder);
        }

        private static void Disassemble(VariableDeclaration declaration, AssemblyBuilder builder)
        {
            if (declaration.TypeIsNotEmpty)
                builder.AppendLine(declaration.ToStatementString().PadRight(StandardStatementWidth)
                                   + declaration.ContextCommentString());
        }

        private static void Disassemble(ControlFlowGraph graph, BasicBlock block, AssemblyBuilder builder)
        {
            var labelIndent = Math.Max(builder.CurrentIndentDepth - 1, 0);
            builder.Append(string.Concat(Enumerable.Repeat(builder.IndentCharacters, labelIndent)));
            builder.Append(block.Name.ToString());
            builder.EndLine(":");
            foreach (var statement in block.Statements)
            {
                Disassemble(graph, graph.LiveVariables?.Before(statement), builder);
                builder.AppendLine(statement.ToStatementString().PadRight(StandardStatementWidth)
                                   + statement.ContextCommentString());
                var insertedDeletes = graph.InsertedDeletes ?? throw new InvalidOperationException("Inserted deletes is null");
                Disassemble(insertedDeletes.After(statement), builder);
                Disassemble(graph.BorrowClaims?.After(statement), builder);
            }
        }

        private static void Disassemble(
            ControlFlowGraph graph,
            BitArray? liveVariables,
            AssemblyBuilder builder)
        {
            if (liveVariables == null || liveVariables.Cast<bool>().All(x => x == false)) return;

            var variables = string.Join(", ", liveVariables.TrueIndexes()
                    .Select(i => FormatVariableName(graph.VariableDeclarations[i])));
            builder.BeginLine("// live: ");
            builder.EndLine(variables);
        }

        private static string FormatVariableName(VariableDeclaration declaration)
        {
            return declaration.Name != null
                ? $"{declaration.Variable}({declaration.Name})"
                : declaration.Variable.ToString();
        }

        private static void Disassemble(
            IReadOnlyList<DeleteStatement> deletes,
            AssemblyBuilder builder)
        {
            foreach (var delete in deletes)
                builder.AppendLine(delete.ToStatementString().PadRight(StandardStatementWidth)
                                   + delete.ContextCommentString());
        }

        private static bool Disassemble(Claims? claims, AssemblyBuilder builder)
        {
            if (claims == null) return false;

            foreach (var claim in claims.AsEnumerable())
            {
                builder.BeginLine("// ");
                builder.EndLine(claim.ToString());
            }

            return claims.Any();
        }

        private static void Disassemble(FieldDeclaration field, AssemblyBuilder builder)
        {
            var binding = field.IsMutableBinding ? "var" : "let";
            builder.AppendLine($"{binding} {field.Name}: {field.Type};");
        }
    }
}
