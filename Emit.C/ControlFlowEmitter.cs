using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.CompareInstructionOperator;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.NumericInstructionOperator;

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

            foreach (var declaration in cfg.VariableDeclarations.Where(v => v.TypeIsNotEmpty))
                EmitVariable(declaration, definitions);

            if (cfg.VariableDeclarations.Any(v => v.TypeIsNotEmpty))
                definitions.BlankLine();

            foreach (var block in cfg.Blocks)
                EmitBlock(block, definitions);
        }

        private void EmitVariable(VariableDeclaration declaration, CCodeBuilder code)
        {
            Requires.That(nameof(declaration), declaration.TypeIsNotEmpty, "tried to look up variable that does not exist");
            var initializer = declaration.IsParameter ? $" = {nameMangler.Mangle(declaration.Name!)}" : "";
            code.AppendLine($"{typeConverter.Convert(declaration.Type)} _{declaration.Variable.Name}{initializer}; // {declaration}");
        }

        private void EmitBlock(Block block, CCodeBuilder code)
        {
            code.AppendLine($"bb{block.Number}:");
            code.BeginBlock();
            foreach (var instruction in block.Instructions)
            {
                EmitInstruction(instruction, code);
            }
            code.EndBlock();
        }

        private void EmitInstruction(Instruction instruction, CCodeBuilder code)
        {
            code.AppendLine("// " + instruction);
            switch (instruction)
            {
                default:
                    throw ExhaustiveMatch.Failed(instruction);
                case InstructionWithResult instructionWithResult:
                    EmitInstructionWithResult(instructionWithResult, code);
                    break;
                case CallInstruction call:
                {
                    if (!(call.ResultPlace is null))
                        EmitResultPlace(call.ResultPlace, code);

                    var mangledName = nameMangler.Mangle(call.Function);
                    var arguments = call.Arguments.Select(ConvertValue);
                    code.AppendLine($"{mangledName}__{call.Arity}({string.Join(", ", arguments)});");
                }
                break;
                case CallVirtualInstruction callVirtual:
                {
                    if (!(callVirtual.ResultPlace is null))
                        EmitResultPlace(callVirtual.ResultPlace, code);

                    var self = ConvertValue(callVirtual.Self);
                    var mangledName = nameMangler.Mangle(callVirtual.Function);
                    var arity = callVirtual.Arity + 1;
                    var arguments = callVirtual.Arguments.Select(ConvertValue).Prepend(self);
                    code.AppendLine($"{self}._vtable->{mangledName}__{arity}({string.Join(", ", arguments)});");
                }
                break;
            }
        }

        private void EmitResultPlace(Place resultPlace, CCodeBuilder code)
        {
            code.Append($"{ConvertPlace(resultPlace)} = ");
        }

        private void EmitInstructionWithResult(InstructionWithResult instruction, CCodeBuilder code)
        {
            EmitResultPlace(instruction.ResultPlace, code);
            switch (instruction)
            {
                default:
                    throw ExhaustiveMatch.Failed(instruction);
                case AssignmentInstruction assignment:
                    code.AppendLine($"{ConvertValue(assignment.Operand)};");
                    break;
                case CompareInstruction compare:
                {
                    var left = ConvertValue(compare.LeftOperand);
                    var right = ConvertValue(compare.RightOperand);
                    var operationType = ConvertSimpleType(compare.Type);
                    string @operator;
                    switch (compare.Operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(compare.Operator);
                        case Equal:
                            @operator = "eq";
                            break;
                        case NotEqual:
                            @operator = "ne";
                            break;
                        case LessThan:
                            @operator = "lt";
                            break;
                        case LessThanOrEqual:
                            @operator = "lte";
                            break;
                        case GreaterThan:
                            @operator = "gt";
                            break;
                        case GreaterThanOrEqual:
                            @operator = "gte";
                            break;
                    }

                    code.AppendLine($"{operationType}__{@operator}({left}, {right});");
                }
                break;
                case NumericInstruction numericInstruction:
                {
                    var left = ConvertValue(numericInstruction.LeftOperand);
                    var right = ConvertValue(numericInstruction.RightOperand);
                    var operationType = ConvertSimpleType(numericInstruction.Type);
                    string @operator;
                    switch (numericInstruction.Operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(numericInstruction.Operator);
                        case Add:
                            @operator = "add";
                            break;
                        case Subtract:
                            @operator = "sub";
                            break;
                        case Multiply:
                            @operator = "mul";
                            break;
                        case Divide:
                            @operator = "div";
                            break;
                    }

                    code.AppendLine($"{operationType}__{@operator}({left}, {right});");
                }
                break;
                case LoadIntegerInstruction loadInteger:
                    code.AppendLine($"({ConvertSimpleType(loadInteger.Type)}){{{loadInteger.Value}}};");
                    break;
                case LoadBoolInstruction loadBool:
                {
                    var booleanValue = loadBool.Value ? 1 : 0;
                    code.AppendLine($"({ConvertSimpleType(DataType.Bool)}){{{booleanValue}}};");
                }
                break;
                case LoadStringInstruction loadString:
                {
                    var constantLength = StringConstant.GetByteLength(loadString.Value);
                    const string selfArgument = "(String){&String___vtable, malloc(sizeof(String___Self))}";
                    var sizeArgument = $"(_size){{{constantLength}}}";
                    var bytesArgument = $"(_size){{(uintptr_t)u8\"{loadString.Value.Escape()}\"}}";
                    code.AppendLine($"String___new__3({selfArgument}, {sizeArgument}, {bytesArgument});");
                }
                break;
                case LoadNoneInstruction loadNone:
                    switch (loadNone.Type.Referent)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(loadNone.Type.Referent);
                        case NeverType _: // `never?` is always none
                        case OptionalType _: // `T??`
                        case AnyType _: // `Any?`
                            throw new NotImplementedException();
                        case UserObjectType userObjectType:
                        {
                            var typeName = nameMangler.Mangle(userObjectType);
                            code.AppendLine($"({loadNone.Type}){{&{typeName}___vtable, NULL}};");
                        }
                        break;
                        case SimpleType simpleType:
                        {
                            var typeName = ConvertSimpleType(simpleType);
                            code.AppendLine($"_opt__{typeName}__none;");
                        }
                        break;
                        case UnknownType _:
                            throw new InvalidOperationException("Instruction uses unknown type (Load None)");
                        case VoidType _:
                            throw new InvalidOperationException("Instruction uses invalid type `void?` (Load None)");
                    }
                    break;
                case NegateInstruction negate:
                {
                    var operand = ConvertValue(negate.Operand);
                    var operationType = ConvertSimpleType(negate.Type);

                    code.AppendLine($"{operationType}__neg({operand});");
                }
                break;
                case ConvertInstruction convert:
                {
                    var valueToConvert = ConvertValue(convert.Operand);
                    var fromType = ConvertSimpleType(convert.FromType);
                    var toType = ConvertSimpleType(convert.ToType);
                    code.AppendLine($"_convert__{fromType}__{toType}({valueToConvert});");
                }
                break;
                case FieldAccessInstruction fieldAccess:
                {
                    var fieldName = nameMangler.Mangle(fieldAccess.FieldName);
                    code.AppendLine($"{ConvertValue(fieldAccess.Operand)}._self->{fieldName};");
                }
                break;

                //case BinaryOperation binaryOperation:
                //{
                //    var left = ConvertValue(binaryOperation.LeftOperand);
                //    var right = ConvertValue(binaryOperation.RightOperand);
                //    var operationType = nameMangler.Mangle(binaryOperation.Type);
                //    string @operator;
                //    switch (binaryOperation.Operator)
                //    {
                //        default:
                //            throw ExhaustiveMatch.Failed(binaryOperation.Operator);
                //        // If a binary operator was emitted for a boolean operation,
                //        // then it doesn't short circuit, we just call the function
                //        case BinaryOperator.And:
                //            @operator = "and";
                //            break;
                //        case BinaryOperator.Or:
                //            @operator = "or";
                //            break;
                //case BinaryOperator.DotDot:
                //    @operator = "dotdot";
                //    break;
                //        case BinaryOperator.LessThanDotDot:
                //            @operator = "ltdotdot";
                //            break;
                //        case BinaryOperator.DotDotLessThan:
                //            @operator = "dotdotlt";
                //            break;
                //        case BinaryOperator.LessThanDotDotLessThan:
                //            @operator = "ltdotdotlt";
                //            break;
                //    }
                //    return $"{operationType}__{@operator}({left}, {right})";
                //}
                //case IfStatement ifStatement:
                //    code.AppendLine($"if({ConvertValue(ifStatement.Condition)}._value) goto {ifStatement.ThenBlock}; else goto {ifStatement.ElseBlock};");
                //    break;
                //case GotoStatement gotoStatement:
                //    code.AppendLine($"goto {gotoStatement.GotoBlock};");
                //    break;
                //case ReturnStatement _:
                //    code.AppendLine(voidReturn ? "return;" : "return _result;");
                //    break;
                //case ActionStatement action:
                //    code.AppendLine(ConvertValue(action.Value) + ";");
                //    break;
                //case DeleteStatement deleteStatement:
                //{
                //    var self = ConvertValue(deleteStatement.Place);
                //    var typeName = nameMangler.Mangle(deleteStatement.Type);
                //    // TODO once deletes are implemented, call them
                //    //code.AppendLine($"{self}._vtable->{typeName}___delete__1({self});");
                //    code.AppendLine($"free({self}._self);");
                //    break;
                //}
                //case ConstructorCall constructorCall:
                //{
                //    var typeName = nameMangler.Mangle(constructorCall.Type);
                //    var selfArgument = $"({typeName}){{&{typeName}___vtable, malloc(sizeof({typeName}___Self))}}";
                //    var arguments = selfArgument.YieldValue().Concat(constructorCall.Arguments.Select(ConvertValue));
                //    return $"{typeName}___new__{constructorCall.Arity}({string.Join(", ", arguments)})";
                //}
                //case DeclaredValue declaredValue:
                //    return nameMangler.Mangle(declaredValue.Name);
                //case ConstructSome constructSome:
                //{
                //    var someValue = ConvertValue(constructSome.Value);
                //    if (constructSome.Type.Referent is ReferenceType)
                //        return someValue;
                //    else
                //    {
                //        var typeName = nameMangler.Mangle(constructSome.Type.Referent);
                //        return $"_opt__{typeName}__Some({someValue})";
                //    }
                //}
            }
        }

        private string ConvertPlace(Place place)
        {
            switch (place)
            {
                default:
                    throw ExhaustiveMatch.Failed(place);
                case VariablePlace reference:
                    return "_" + reference.Variable.Name;
                case FieldPlace fieldAccess:
                    var fieldName = nameMangler.Mangle(fieldAccess.Field.UnqualifiedName);
                    return $"{ConvertValue(fieldAccess.Target)}._self->{fieldName}";
            }
        }

        private static string ConvertValue(Operand value)
        {
            switch (value)
            {
                default:
                    //throw new NotImplementedException();
                    throw ExhaustiveMatch.Failed(value);
                case VariableReference variableReference:
                    return "_" + variableReference.Variable.Name;
            }
        }

        private string ConvertSimpleType(SimpleType type)
        {
            return nameMangler.Mangle(type);
        }
    }
}
