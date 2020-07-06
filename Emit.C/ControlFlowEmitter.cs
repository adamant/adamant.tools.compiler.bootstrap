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

            if (callable is ConstructorDeclaration constructor)
            {
                foreach (var fieldInitialization in constructor.FieldInitializations)
                    EmitFieldInitialization(fieldInitialization, definitions);
                if (constructor.FieldInitializations.Any()) definitions.BlankLine();
            }

            foreach (var declaration in cfg.VariableDeclarations.Where(v => v.TypeIsNotEmpty))
                EmitVariable(declaration, definitions);

            if (cfg.VariableDeclarations.Any(v => v.TypeIsNotEmpty))
                definitions.BlankLine();

            foreach (var block in cfg.Blocks)
                EmitBlock(block, callable.IsConstructor, definitions);
        }

        private void EmitFieldInitialization(FieldInitialization fieldInitialization, CCodeBuilder code)
        {
            var fieldName = nameMangler.Mangle(fieldInitialization.FieldName);
            var parameterName = nameMangler.Mangle(fieldInitialization.ParameterName);
            // Emit direct access to avoid any issues about whether corresponding variables exist
            code.AppendLine($"_self._self->{fieldName} = {parameterName};");
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

                    string self = "";
                    if (ins.IsMethodCall) self = ConvertOperand(ins.Self!);

                    var mangledName = nameMangler.Mangle(ins.FunctionName);
                    var arguments = ins.Arguments.Select(ConvertOperand);
                    if (ins.IsMethodCall) arguments = arguments.Prepend(self);
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
                    var mangledName = nameMangler.Mangle(ins.Function.UnqualifiedName);
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
                case NewObjectInstruction ins:
                {
                    var mangledName = nameMangler.Mangle(ins.Constructor);
                    var typeName = nameMangler.Mangle(ins.ConstructedType);
                    var selfArgument = $"({typeName}){{&{typeName}___vtable, malloc(sizeof({typeName}___Self))}}";
                    var arguments = ins.Arguments.Select(ConvertOperand).Prepend(selfArgument);
                    code.EndLine($"{mangledName}__{ins.Arity + 1}({string.Join(", ", arguments)});");
                }
                break;
                case AssignmentInstruction ins:
                    code.EndLine($"{ConvertOperand(ins.Operand)};");
                    break;
                case CompareInstruction ins:
                {
                    var left = ConvertOperand(ins.LeftOperand);
                    var right = ConvertOperand(ins.RightOperand);
                    var operationType = ConvertSimpleType(ins.Type);
                    string op = ins.Operator switch
                    {
                        Equal => "eq",
                        NotEqual => "ne",
                        LessThan => "lt",
                        LessThanOrEqual => "lte",
                        GreaterThan => "gt",
                        GreaterThanOrEqual => "gte",
                        _ => throw ExhaustiveMatch.Failed(ins.Operator)
                    };

                    code.EndLine($"{operationType}__{op}({left}, {right});");
                }
                break;
                case NumericInstruction ins:
                {
                    var left = ConvertOperand(ins.LeftOperand);
                    var right = ConvertOperand(ins.RightOperand);
                    var operationType = ConvertSimpleType(ins.Type);
                    string op = ins.Operator switch
                    {
                        Add => "add",
                        Subtract => "sub",
                        Multiply => "mul",
                        Divide => "div",
                        _ => throw ExhaustiveMatch.Failed(ins.Operator)
                    };

                    code.EndLine($"{operationType}__{op}({left}, {right});");
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
                case SomeInstruction ins:
                {
                    var someValue = ConvertOperand(ins.Operand);
                    if (ins.Type.Referent is ReferenceType)
                        code.EndLine(someValue);
                    else
                    {
                        var typeName = nameMangler.Mangle(ins.Type.Referent);
                        code.EndLine($"_opt__{typeName}__Some({someValue});");
                    }
                }
                break;
                case BooleanLogicInstruction ins:
                {
                    var left = ConvertOperand(ins.LeftOperand);
                    var right = ConvertOperand(ins.RightOperand);
                    var operationType = nameMangler.Mangle(DataType.Bool);
                    string op = ins.Operator switch
                    {
                        // If a binary operator was emitted for a boolean operation,
                        // then it doesn't short circuit, we just call the function
                        BooleanLogicOperator.And => "and",
                        BooleanLogicOperator.Or => "or",
                        _ => throw ExhaustiveMatch.Failed(ins.Operator)
                    };

                    code.EndLine($"{operationType}__{op}({left}, {right});");
                }
                break;


                //case DeleteStatement deleteStatement:
                //{
                //    var self = ConvertValue(deleteStatement.Place);
                //    var typeName = nameMangler.Mangle(deleteStatement.Type);
                //    // TODO once deletes are implemented, call them
                //    //code.AppendLine($"{self}._vtable->{typeName}___delete__1({self});");
                //    code.AppendLine($"free({self}._self);");
                //    break;
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
            return value switch
            {
                VariableReference op => "_" + op.Variable.Name,
                _ => throw ExhaustiveMatch.Failed(value)
            };
        }

        private string ConvertSimpleType(SimpleType type)
        {
            return nameMangler.Mangle(type);
        }
    }
}
