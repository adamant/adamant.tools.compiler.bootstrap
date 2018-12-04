using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ControlFlowEmitter : IEmitter<ControlFlowGraph>
    {
        private readonly NameMangler nameMangler;
        private readonly IConverter<DataType> typeConverter;

        public ControlFlowEmitter(
            NameMangler nameMangler,
            IConverter<DataType> typeConverter)
        {
            this.typeConverter = typeConverter;
            this.nameMangler = nameMangler;
        }

        public void Emit(ControlFlowGraph cfg, Code code)
        {
            var definitions = code.Definitions;

            foreach (var variable in cfg.VariableDeclarations.Where(v => v.Exists))
                EmitVariable(variable, definitions);

            if (cfg.VariableDeclarations.Any(v => v.Exists))
                definitions.BlankLine();

            var voidReturn = cfg.ReturnType == DataType.Void;
            foreach (var block in cfg.BasicBlocks)
                EmitBlock(block, voidReturn, definitions);
        }

        private void EmitVariable(LocalVariableDeclaration variable, CCodeBuilder code)
        {
            Requires.That(nameof(variable), variable.Exists, "tried to look up variable that does not exist");
            var initializer = variable.IsParameter ? $" = {nameMangler.Mangle(variable.Name)}" : "";
            var name = variable.Name != null ? " // " + variable.Name : "";
            code.AppendLine($"{typeConverter.Convert(variable.Type)} ₜ{NameOf(variable.Reference)}{initializer};{name}");
        }

        private static string NameOf(VariableReference variable)
        {
            return variable.VariableNumber == 0 ? "result" : variable.VariableNumber.ToString();
        }

        private void EmitBlock(BasicBlock block, bool voidReturn, CCodeBuilder code)
        {
            code.AppendLine($"bb{block.Number}:");
            code.BeginBlock();
            foreach (var statement in block.Statements) EmitStatement(statement, voidReturn, code);
            code.EndBlock();
        }

        private void EmitStatement(Statement statement, bool voidReturn, CCodeBuilder code)
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
                case ActionStatement action:
                    code.AppendLine(ConvertValue(action.Value) + ";");
                    break;
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
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
                    return $"((ₐbyte*)u8\"{utf8BytesConstant.Value.Escape()}\")";
                case FunctionCall functionCall:
                {
                    var mangledName = nameMangler.Mangle(functionCall.FunctionName);
                    var arguments = functionCall.Arguments.Select(ConvertValue);
                    return $"{mangledName}({string.Join(", ", arguments)})";
                }
                case ConstructorCall _:
                    // TODO implement this
                    throw new NotImplementedException();
                case DeclaredValue declaredValue:
                    return nameMangler.Mangle(declaredValue.Name);
                case FieldAccessValue fieldAccess:
                    return $"{ConvertValue(fieldAccess.Expression)}.{nameMangler.Mangle(fieldAccess.Field.UnqualifiedName)}";
                case CopyPlace copyPlace:
                    return ConvertPlace(copyPlace.Place);
                case VirtualFunctionCall virtualCall:
                {
                    var self = ConvertValue(virtualCall.Self);
                    var mangledName = nameMangler.Mangle(virtualCall.FunctionName);
                    var arguments = virtualCall.Arguments.Select(ConvertValue).Prepend(self);
                    return $"{self}.ₐvtable.{mangledName}({string.Join(", ", arguments)})";
                }
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
