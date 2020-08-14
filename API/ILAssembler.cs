using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class ILAssembler
    {
        protected const int StandardStatementWidth = 50;

        public string Disassemble(PackageIL package)
        {
            var builder = new AssemblyBuilder();
            var typeMembers = package.Declarations.OfType<ClassIL>()
                .SelectMany(t => t.Members).ToList();
            foreach (var declaration in package.Declarations.Except(typeMembers))
            {
                Disassemble(declaration, builder);
                builder.BlankLine();
            }

            return builder.Code;
        }

        public void Disassemble(DeclarationIL declaration, AssemblyBuilder builder)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case FunctionIL function:
                    Disassemble(function, builder);
                    break;
                case MethodDeclarationIL method:
                    Disassemble(method, builder);
                    break;
                case ConstructorIL constructor:
                    Disassemble(constructor, builder);
                    break;
                case ClassIL type:
                    Disassemble(type, builder);
                    break;
                case FieldIL field:
                    Disassemble(field, builder);
                    break;
            }
        }

        private static void Disassemble(FunctionIL function, AssemblyBuilder builder)
        {
            var parameters = FormatParameters(function.Parameters);
            builder.BeginLine("fn ");
            builder.Append(Convert(function.Symbol));
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine(builder.IndentCharacters);
            builder.Append("-> ");
            builder.EndLine(function.Symbol.ReturnDataType.ToString());
            if (function.IL != null)
            {
                builder.BeginBlock();
                Disassemble(function.IL, builder);
                builder.EndBlock();
            }
        }

        private static string FormatParameters(IEnumerable<ParameterIL> parameters)
        {
            var formatted = string.Join(", ", parameters.Select(FormatParameter));
            return formatted;
        }

        private static string FormatParameter(ParameterIL parameter)
        {
            var format = parameter.IsMutableBinding ? "var {0}: {1}" : "{0}: {1}";
            return parameter switch
            {
                // TODO what about escaping the name?
                NamedParameterIL param =>
                    string.Format(CultureInfo.InvariantCulture, format, param.Symbol.Name, param.DataType),
                // TODO is this the correct name for self?
                SelfParameterIL param =>
                    string.Format(CultureInfo.InvariantCulture, format, "self", param.DataType),
                FieldParameterIL param => $".{param.InitializeField.Name}",
                _ => throw ExhaustiveMatch.Failed(parameter)
            };
        }

        private static void Disassemble(MethodDeclarationIL method, AssemblyBuilder builder)
        {
            var parameters = FormatParameters(method.Parameters.Prepend(method.SelfParameter));
            builder.BeginLine("fn ");
            builder.Append(Convert(method.Symbol));
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine(builder.IndentCharacters);
            builder.Append("-> ");
            builder.EndLine(method.Symbol.ReturnDataType.ToString());
            if (method.IL != null)
            {
                builder.BeginBlock();
                Disassemble(method.IL, builder);
                builder.EndBlock();
            }
        }

        private static void Disassemble(ConstructorIL constructor, AssemblyBuilder builder)
        {
            var parameters = FormatParameters(constructor.Parameters);
            builder.BeginLine("fn ");
            builder.Append(Convert(constructor.Symbol));
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine(builder.IndentCharacters);
            builder.Append("-> ");
            builder.EndLine(constructor.Symbol.ContainingSymbol.DeclaresDataType.ToString());
            builder.BeginBlock();
            Disassemble(constructor.IL, builder);
            builder.EndBlock();
        }

        private void Disassemble(ClassIL @class, AssemblyBuilder builder)
        {
            builder.BeginLine("class ");
            builder.Append(Convert(@class.Symbol));
            builder.EndLine();
            builder.BeginBlock();
            foreach (var member in @class.Members)
            {
                builder.DeclarationSeparatorLine();
                Disassemble(member, builder);
            }
            builder.EndBlock();
        }

        private static void Disassemble(ControlFlowGraph cfg, AssemblyBuilder builder)
        {
            if (cfg is null)
            {
                builder.AppendLine("// Control Flow Graph not available");
                return;
            }

            foreach (var declaration in cfg.LocalVariables)
                Disassemble(declaration, builder);

            if (cfg.LocalVariables.Any(v => v.TypeIsNotEmpty))
                builder.BlankLine();

            //if (Disassemble(cfg.BorrowClaims?.ParameterClaims, builder))
            //    builder.BlankLine();

            foreach (var block in cfg.Blocks)
                Disassemble(cfg, block, builder);
        }

        private static void Disassemble(VariableDeclaration declaration, AssemblyBuilder builder)
        {
            if (declaration.TypeIsNotEmpty)
                builder.AppendLine(declaration.ToStatementString().PadRight(StandardStatementWidth)
                                   + declaration.ContextCommentString());
        }

        private static void Disassemble(ControlFlowGraph cfg, Block block, AssemblyBuilder builder)
        {
            var labelIndent = Math.Max(builder.CurrentIndentDepth - 1, 0);
            builder.Append(string.Concat(Enumerable.Repeat(builder.IndentCharacters, labelIndent)));
            builder.Append("bb");
            builder.Append(block.Number.ToString(CultureInfo.InvariantCulture));
            builder.EndLine(":");
            foreach (var instruction in block.Instructions)
            {
                _ = cfg;
                //Disassemble(cfg, cfg.LiveVariables?.Before(instruction), builder);
                builder.AppendLine(instruction.ToInstructionString().PadRight(StandardStatementWidth)
                                   + instruction.ContextCommentString());
                //var insertedDeletes = cfg.InsertedDeletes;
                //var insertedDeletes = cfg.InsertedDeletes ?? throw new InvalidOperationException("Inserted deletes is null");
                //Disassemble(insertedDeletes.After(instruction), builder);
                //Disassemble(cfg.BorrowClaims?.After(instruction), builder);
            }

            var terminator = block.Terminator;
            builder.AppendLine(terminator.ToInstructionString().PadRight(StandardStatementWidth));
        }

        //private static void Disassemble(
        //    ControlFlowGraph cfg,
        //    BitArray? liveVariables,
        //    AssemblyBuilder builder)
        //{
        //    if (liveVariables is null || liveVariables.Cast<bool>().All(x => x == false)) return;

        //    var variables = string.Join(", ", liveVariables.TrueIndexes()
        //            .Select(i => FormatVariableName(cfg.VariableDeclarations[i])));
        //    builder.BeginLine("// live: ");
        //    builder.EndLine(variables);
        //}

        //private static string FormatVariableName(VariableDeclaration declaration)
        //{
        //    return declaration.Name != null
        //        ? $"{declaration.Variable}({declaration.Name})"
        //        : declaration.Variable.ToString();
        //}

        //private static void Disassemble(
        //    IReadOnlyList<DeleteStatement> deletes,
        //    AssemblyBuilder builder)
        //{
        //    foreach (var delete in deletes)
        //        builder.AppendLine(delete.ToStatementString().PadRight(StandardStatementWidth)
        //                           + delete.ContextCommentString());
        //}

        //private static bool Disassemble(Claims? claims, AssemblyBuilder builder)
        //{
        //    if (claims is null) return false;

        //    foreach (var claim in claims.AsEnumerable())
        //    {
        //        builder.BeginLine("// ");
        //        builder.EndLine(claim.ToString());
        //    }

        //    return claims.Any();
        //}

        private static void Disassemble(FieldIL field, AssemblyBuilder builder)
        {
            var binding = field.IsMutableBinding ? "var" : "let";
            builder.AppendLine($"{binding} {field.Symbol.Name}: {field.DataType};");
        }

        private static string Convert(TypeSymbol symbol)
        {
            if (symbol.ContainingSymbol is null) return Convert(symbol.Name);
            return Convert(symbol.ContainingSymbol) + "." + Convert(symbol.Name);
        }

        private static string Convert(FunctionSymbol symbol)
        {
            var name = symbol.ContainingSymbol switch
            {
                NamespaceOrPackageSymbol ns => Convert(ns),
                ObjectTypeSymbol sym => Convert(sym),
                _ => throw new NotSupportedException(symbol.GetType().ToString())
            };

            name += "."+ Convert(symbol.Name);
            return name;
        }

        private static string Convert(MethodSymbol symbol)
        {
            return Convert(symbol.ContainingSymbol) + "." + Convert(symbol.Name);
        }

        private static string Convert(ConstructorSymbol symbol)
        {
            var name = Convert(symbol.ContainingSymbol) + ".new";
            if (!(symbol.Name is null))
                name += "." + Convert(symbol.Name);
            return name;
        }

        private static string Convert(NamespaceOrPackageSymbol symbol)
        {
            return symbol switch
            {
                NamespaceSymbol sym => Convert(sym.ContainingSymbol) + "." + Convert(sym.Name),
                PackageSymbol sym => sym.Name + "::",
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
        }

        private static string Convert(TypeName name)
        {
            return name.ToString();
        }
    }
}
