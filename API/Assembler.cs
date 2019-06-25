using System;
using System.Collections;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class Assembler
    {
        protected const int StandardStatementWidth = 50;

        public string Disassemble(Package package)
        {
            var builder = new AssemblyBuilder();
            var typeMembers = package.Declarations.OfType<TypeDeclaration>()
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
                case TypeDeclaration type:
                    Disassemble(type, builder);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        public void Disassemble(FunctionDeclaration function, AssemblyBuilder builder)
        {
            var functionControlFlow = function.ControlFlow;
            var parameters = FormatParameters(functionControlFlow);
            builder.BeginLine("fn ");
            builder.Append(function.FullName.ToString());
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine(builder.IndentCharacters);
            builder.Append("-> ");
            builder.EndLine(function.ReturnType.ToString());
            builder.BeginBlock();
            Disassemble(function.ControlFlow, builder);
            builder.EndBlock();
        }

        private static string FormatParameters(ControlFlowGraph controlFlow)
        {
            var parameters = string.Join(", ", controlFlow.Parameters.Select(FormatParameter));
            return parameters;
        }

        private static string FormatParameter(VariableDeclaration parameter)
        {
            var format = parameter.IsMutableBinding ? "var {0}({1}): {2}" : "{0}({1}): {2}";
            return string.Format(format, parameter.Variable, parameter.Name, parameter.Type);
        }

        public void Disassemble(ConstructorDeclaration constructor, AssemblyBuilder builder)
        {
            var parameters = string.Join(", ", constructor.ControlFlow.Parameters.Select(FormatParameter));
            builder.BeginLine("fn ");
            builder.Append(constructor.FullName.ToString());
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

        public void Disassemble(TypeDeclaration type, AssemblyBuilder builder)
        {
            builder.BeginLine("type ");
            builder.Append(type.FullName.ToString());
            builder.EndLine();
            builder.BeginBlock();
            foreach (var member in type.Members)
            {
                builder.DeclarationSeparatorLine();
                Disassemble(member, builder);
            }
            builder.EndBlock();
        }

        private void Disassemble(ControlFlowGraph graph, AssemblyBuilder builder)
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

        private void Disassemble(VariableDeclaration declaration, AssemblyBuilder builder)
        {
            if (declaration.TypeIsNotEmpty)
                builder.AppendLine(declaration.ToStatementString().PadRight(StandardStatementWidth) + declaration.ContextCommentString());
        }

        private void Disassemble(ControlFlowGraph graph, BasicBlock block, AssemblyBuilder builder)
        {
            var labelIndent = Math.Max(builder.CurrentIndentDepth - 1, 0);
            builder.Append(string.Concat(Enumerable.Repeat(builder.IndentCharacters, labelIndent)));
            builder.Append(block.Name.ToString());
            builder.EndLine(":");
            foreach (var statement in block.Statements)
            {
                Disassemble(graph, graph.LiveVariables?.Before(statement), builder);
                builder.AppendLine(statement.ToStatementString().PadRight(StandardStatementWidth) + statement.ContextCommentString());
                Disassemble(graph.BorrowClaims?.After(statement), builder);
            }
        }

        private void Disassemble(
            ControlFlowGraph graph,
            BitArray liveVariables,
            AssemblyBuilder builder)
        {
            if (liveVariables == null || liveVariables.Cast<bool>().All(x => x == false)) return;

            var variables = string.Join(", ", liveVariables.TrueIndexes()
                                .Select(i => FormatVariableName(graph.VariableDeclarations[i])));
            builder.BeginLine("// live: ");
            builder.EndLine(variables);
        }

        private string FormatVariableName(VariableDeclaration declaration)
        {
            return declaration.Name != null
                ? $"{declaration.Variable}({declaration.Name})"
                : declaration.Variable.ToString();
        }

        private bool Disassemble(Claims claims, AssemblyBuilder builder)
        {
            if (claims == null) return false;

            foreach (var claim in claims.AsEnumerable())
            {
                builder.BeginLine("// ");
                builder.EndLine(claim.ToString());
            }

            return claims.Any();
        }
    }
}
