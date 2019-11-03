# Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage

The Adamant Intermediate Language (IL) is both an intermediate representation (IR) for the compiler and the format for distributing machine independent Adamant libraries.

## Structure

The Adamant IL is loosely based on the Rust MIR. It uses a set of local variables whose names have been erased. Additionally, the compiler may introduce many temporary variables and these are freely mixed into the declared variable.

## Syntax

Each function or method has a meta data declaration.

```IL
fn Namespace.Optional_Class_Name::Method_name(argument: int32, another_arg: Type) -> Namespace.Return_Type
{
    // Bindings
    // The variable _0 is implicitly defined as the return type with a non-mutable binding
    // The variables _1 to _n where n is the number of arguments refer to the arguments. Note that the argument names exist for meta-data purposes only and aren't really used in the IL
    let _3: int32 // notice the full qualifying of even the default size
    var _4: int32 // `var` indicates a mutable binding

    bb0: // basic blocks form a control flow graph
    {
        _3 = const 4:int32 // constant types are explicit
        _2 = _1 + _3 // Type not specified, but the three types must be the same and must be valid for basic add

    }
}
```

## Assembly

If desired a more flexible assembly language can be provided for human use. This would provide the following conveniences.

* Use of arguments by name instead of index
* Declaration of named variables
* Declarations of variables at any point on the code
* Assignment of initial values as part of declaration statements;
* Comments introduced with double slash

## Namespaces

Like the CIL, the Adamant IL doesn't contain namespaces. Rather, each entity name is its fully qualified name. This means that a package can never expose an empty namespace.
