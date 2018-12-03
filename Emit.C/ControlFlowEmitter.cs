using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ControlFlowEmitter : IEmitter<ControlFlowGraph>
    {
        [NotNull] private readonly NameMangler nameMangler;
        [NotNull] private readonly IConverter<DataType> typeConverter;

        public ControlFlowEmitter(
            [NotNull] NameMangler nameMangler,
            [NotNull] IConverter<DataType> typeConverter)
        {
            this.typeConverter = typeConverter;
            this.nameMangler = nameMangler;
        }

        public void Emit([NotNull] ControlFlowGraph cfg, [NotNull] Code code)
        {
            var definitions = code.Definitions;

            foreach (var variable in cfg.VariableDeclarations.Where(v => v.Exists))
                Emit(variable, definitions);

            if (cfg.VariableDeclarations.Any(v => v.Exists))
                definitions.BlankLine();

            // TODO assign parameters to temp variables?

            var voidReturn = cfg.ReturnType == DataType.Void;
            foreach (var block in cfg.BasicBlocks)
                EmitBlock(block, voidReturn, definitions);
        }

        private void Emit([NotNull] LocalVariableDeclaration variable, [NotNull] CCodeBuilder code)
        {
            Requires.That(nameof(variable), variable.Exists);
            var initializer = variable.IsParameter ? $" = {nameMangler.Mangle(variable.Name.NotNull())}" : "";
            code.AppendLine($"{typeConverter.Convert(variable.Type)} ₜ{NameOf(variable.Reference)}{initializer};");
        }

        private static string NameOf([NotNull] VariableReference variable)
        {
            return variable.VariableNumber == 0 ? "result" : variable.VariableNumber.ToString();
        }

        private void EmitBlock([NotNull] BasicBlock block, bool voidReturn, [NotNull] CCodeBuilder code)
        {
            code.AppendLine($"bb{block.Number}:");
            code.BeginBlock();
            foreach (var statement in block.Statements)
            {
                code.AppendLine("// " + statement);
                switch (statement)
                {
                    case ReturnStatement _:
                        code.AppendLine(voidReturn ? "return;" : "return ₜresult;");
                        break;
                    case AssignmentStatement assignment:
                        code.AppendLine(
                            $"{ConvertPlace(assignment.Place)} = {ConvertValue(assignment.Value)};");
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(statement);
                }
            }
            code.EndBlock();
        }

        private string ConvertPlace(Place place)
        {
            switch (place)
            {
                case VariableReference variable:
                    return "ₜ" + NameOf(variable);
                default:
                    throw NonExhaustiveMatchException.For(place);
            }
        }

        private string ConvertValue(Value value)
        {
            switch (value)
            {
                case IntegerConstant integer:
                    return $"({ConvertType(integer.Type)}){{{integer.Value}}}";
                case Utf8BytesConstant utf8BytesConstant:
                    return $"((ₐbyte const *)u8\"{utf8BytesConstant.Value.Escape()}\")";
                //case VariableReference variable:
                //    return "ₜ" + NameOf(variable);
                case FunctionCall callStatement:
                    var mangledName = nameMangler.Mangle(callStatement.FunctionName);
                    var arguments = callStatement.Arguments.Select(ConvertValue);
                    return $"{mangledName}({string.Join(", ", arguments)});";
                case ConstructorCall newObjectStatement:
                    // TODO implement this
                    throw new NotImplementedException();
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private string ConvertType(DataType type)
        {
            switch (type)
            {
                case ObjectType objectType:
                    return nameMangler.Mangle(objectType);
                case SimpleType simpleType:
                    return nameMangler.Mangle(simpleType.Name);
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }
    }
}
