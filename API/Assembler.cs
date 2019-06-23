using System;
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
        public string Disassemble(Package package)
        {
            var builder = new AssemblyBuilder();
            foreach (var declaration in package.Declarations)
            {
                switch (declaration)
                {
                    case FunctionDeclaration _:
                    case TypeDeclaration _:
                        Disassemble(declaration, builder);
                        break;
                    case ConstructorDeclaration _:
                        // Constructors are emitted as part of the type declaration
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(declaration);
                }

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
            var parameters = string.Join(", ", function.Parameters.Select(FormatParameter));
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

        private static string FormatParameter(Parameter parameter)
        {
            var format = parameter.MutableBinding ? "mut {0}: {1}" : "{0}: {1}";
            return string.Format(format, parameter.Name, parameter.Type);
        }

        public void Disassemble(ConstructorDeclaration constructor, AssemblyBuilder builder)
        {
            var parameters = string.Join(", ", constructor.Parameters.Select(FormatParameter));
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

        private void Disassemble(ControlFlowGraph controlFlow, AssemblyBuilder builder)
        {
            if (controlFlow == null)
            {
                builder.AppendLine("// Control Flow Graph not available");
                return;
            }

            foreach (var declaration in controlFlow.VariableDeclarations)
                Disassemble(declaration, builder);

            if (controlFlow.VariableDeclarations.Any(v => v.TypeIsNotEmpty))
                builder.BlankLine();

            if (Disassemble(controlFlow.BorrowClaims?.ParameterClaims, builder))
                builder.BlankLine();

            foreach (var block in controlFlow.BasicBlocks)
                Disassemble(block, controlFlow.BorrowClaims, builder);
        }

        private void Disassemble(LocalVariableDeclaration declaration, AssemblyBuilder builder)
        {
            if (declaration.TypeIsNotEmpty)
                builder.AppendLine(declaration.ToString());
        }

        private void Disassemble(
            BasicBlock block,
            StatementClaims borrowClaims,
            AssemblyBuilder builder)
        {

            var labelIndent = Math.Max(builder.CurrentIndentDepth - 1, 0);
            builder.Append(string.Concat(Enumerable.Repeat(builder.IndentCharacters, labelIndent)));
            builder.Append(block.Name.ToString());
            builder.EndLine(":");
            foreach (var statement in block.Statements)
            {
                builder.AppendLine(statement.ToString());
                Disassemble(borrowClaims?.After(statement), builder);
            }
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
