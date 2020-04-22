using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.CompareInstructionOperator;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.NumericInstructionOperator;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ControlFlowEmitter : IEmitter<ICallableDeclaration>
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

        public void Emit(ICallableDeclaration callable, Code code)
        {
            var cfg = callable.IL!;
            var definitions = code.Definitions;

            foreach (var declaration in cfg.VariableDeclarations.Where(v => v.TypeIsNotEmpty))
                EmitVariable(declaration, definitions);

            if (cfg.VariableDeclarations.Any(v => v.TypeIsNotEmpty))
                definitions.BlankLine();

            foreach (var block in cfg.Blocks)
                EmitBlock(block, callable.IsConstructor, definitions);
        }

        private void EmitVariable(VariableDeclaration declaration, CCodeBuilder code)
        {
            Requires.That(nameof(declaration), declaration.TypeIsNotEmpty, "tried to look up variable that does not exist");
            var initializer = declaration.IsParameter ? $" = {nameMangler.Mangle(declaration.Name!)}" : "";
            code.AppendLine($"{typeConverter.Convert(declaration.Type)} _{declaration.Variable.Name}{initializer}; // {declaration}");
        }

        private void EmitBlock(Block block, bool isConstructor, CCodeBuilder code)
        {
            code.AppendLine($"bb{block.Number}:");
            code.BeginBlock();
            foreach (var instruction in block.Instructions)
                EmitInstruction(instruction, code);

            EmitInstruction(block.Terminator, isConstructor, code);
            code.EndBlock();
        }

        private void EmitInstruction(Instruction instruction, CCodeBuilder code)
        {
            code.AppendLine("// " + instruction);
            switch (instruction)
            {
                default:
                    throw ExhaustiveMatch.Failed(instruction);
                case InstructionWithResult ins:
                    EmitInstructionWithResult(ins, code);
                    break;
                case CallInstruction ins:
                {
                    if (!(ins.ResultPlace is null))
                        EmitResultPlace(ins.ResultPlace, code);
                    else
                        code.BeginLine("");

                    var mangledName = nameMangler.Mangle(ins.Function);
                    var arguments = ins.Arguments.Select(ConvertOperand);
                    code.EndLine($"{mangledName}__{ins.Arity}({string.Join(", ", arguments)});");
                }
                break;
                case CallVirtualInstruction ins:
                {
                    if (!(ins.ResultPlace is null))
                        EmitResultPlace(ins.ResultPlace, code);
                    else
                        code.BeginLine("");

                    var self = ConvertOperand(ins.Self);
                    var mangledName = nameMangler.Mangle(ins.Function);
                    var arity = ins.Arity + 1;
                    var arguments = ins.Arguments.Select(ConvertOperand).Prepend(self);
                    code.EndLine($"{self}._vtable->{mangledName}__{arity}({string.Join(", ", arguments)});");
                }
                break;
            }
        }

        private void EmitResultPlace(Place resultPlace, CCodeBuilder code)
        {
            code.BeginLine($"{ConvertPlace(resultPlace)} = ");
        }

        private void EmitInstructionWithResult(InstructionWithResult instruction, CCodeBuilder code)
        {
            EmitResultPlace(instruction.ResultPlace, code);
            switch (instruction)
            {
                default:
                    throw ExhaustiveMatch.Failed(instruction);
                case AssignmentInstruction ins:
                    code.EndLine($"{ConvertOperand(ins.Operand)};");
                    break;
                case CompareInstruction ins:
                {
                    var left = ConvertOperand(ins.LeftOperand);
                    var right = ConvertOperand(ins.RightOperand);
                    var operationType = ConvertSimpleType(ins.Type);
                    string @operator;
                    switch (ins.Operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(ins.Operator);
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

                    code.EndLine($"{operationType}__{@operator}({left}, {right});");
                }
                break;
                case NumericInstruction ins:
                {
                    var left = ConvertOperand(ins.LeftOperand);
                    var right = ConvertOperand(ins.RightOperand);
                    var operationType = ConvertSimpleType(ins.Type);
                    string @operator;
                    switch (ins.Operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(ins.Operator);
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

                    code.EndLine($"{operationType}__{@operator}({left}, {right});");
                }
                break;
                case LoadIntegerInstruction ins:
                    code.EndLine($"({ConvertSimpleType(ins.Type)}){{{ins.Value}}};");
                    break;
                case LoadBoolInstruction ins:
                {
                    var booleanValue = ins.Value ? 1 : 0;
                    code.EndLine($"({ConvertSimpleType(DataType.Bool)}){{{booleanValue}}};");
                }
                break;
                case LoadStringInstruction ins:
                {
                    var constantLength = StringConstant.GetByteLength(ins.Value);
                    const string selfArgument = "(String){&String___vtable, malloc(sizeof(String___Self))}";
                    var sizeArgument = $"(_size){{{constantLength}}}";
                    var bytesArgument = $"(_size){{(uintptr_t)u8\"{ins.Value.Escape()}\"}}";
                    code.EndLine($"String___new__3({selfArgument}, {sizeArgument}, {bytesArgument});");
                }
                break;
                case LoadNoneInstruction ins:
                    switch (ins.Type.Referent)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(ins.Type.Referent);
                        case NeverType _: // `never?` is always none
                        case OptionalType _: // `T??`
                        case AnyType _: // `Any?`
                            throw new NotImplementedException();
                        case UserObjectType userObjectType:
                        {
                            var typeName = nameMangler.Mangle(userObjectType);
                            code.EndLine($"({ins.Type}){{&{typeName}___vtable, NULL}};");
                        }
                        break;
                        case SimpleType simpleType:
                        {
                            var typeName = ConvertSimpleType(simpleType);
                            code.EndLine($"_opt__{typeName}__none;");
                        }
                        break;
                        case UnknownType _:
                            throw new InvalidOperationException("Instruction uses unknown type (Load None)");
                        case VoidType _:
                            throw new InvalidOperationException("Instruction uses invalid type `void?` (Load None)");
                    }
                    break;
                case NegateInstruction ins:
                {
                    var operand = ConvertOperand(ins.Operand);
                    var operationType = ConvertSimpleType(ins.Type);

                    code.EndLine($"{operationType}__neg({operand});");
                }
                break;
                case ConvertInstruction ins:
                {
                    var valueToConvert = ConvertOperand(ins.Operand);
                    var fromType = ConvertSimpleType(ins.FromType);
                    var toType = ConvertSimpleType(ins.ToType);
                    code.EndLine($"_convert__{fromType}__{toType}({valueToConvert});");
                }
                break;
                case FieldAccessInstruction ins:
                {
                    var fieldName = nameMangler.Mangle(ins.FieldName);
                    code.EndLine($"{ConvertOperand(ins.Operand)}._self->{fieldName};");
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

        private static void EmitInstruction(TerminatorInstruction instruction, bool isConstructor, CCodeBuilder code)
        {
            code.AppendLine("// " + instruction);
            switch (instruction)
            {
                default:
                    throw ExhaustiveMatch.Failed(instruction);
                case GotoInstruction ins:
                    code.AppendLine($"goto bb{ins.BlockNumber};");
                    break;
                case IfInstruction ins:
                    code.AppendLine($"if({ConvertOperand(ins.Condition)}._value) goto bb{ins.ThenBlockNumber}; else goto bb{ins.ElseBlockNumber};");
                    break;
                case ReturnValueInstruction ins:
                    code.AppendLine($"return {ConvertOperand(ins.Value)};");
                    break;
                case ReturnVoidInstruction _:
                    code.AppendLine(isConstructor ? "return _0;" : "return;");
                    break;
            }
        }

        private string ConvertPlace(Place place)
        {
            switch (place)
            {
                default:
                    throw ExhaustiveMatch.Failed(place);
                case VariablePlace p:
                    return "_" + p.Variable.Name;
                case FieldPlace p:
                    var fieldName = nameMangler.Mangle(p.Field.UnqualifiedName);
                    return $"{ConvertOperand(p.Target)}._self->{fieldName}";
            }
        }

        private static string ConvertOperand(Operand value)
        {
            switch (value)
            {
                default:
                    throw ExhaustiveMatch.Failed(value);
                case VariableReference op:
                    return "_" + op.Variable.Name;
            }
        }

        private string ConvertSimpleType(SimpleType type)
        {
            return nameMangler.Mangle(type);
        }
    }
}
