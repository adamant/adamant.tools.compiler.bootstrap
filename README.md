# Adamant Bootstrap Compiler

A bootstrap compiler for the [Adamant language](http://adamant-lang.org).  That is, a compiler for a subset of the language that will be used to write the Adamant compiler in Adamant. The compiler currently compiles a small subset of the Adamant language to C.

## Project Status: Alpha Active

The compiler is under active development. It is in a very early stage, and there are likely issues and limitations. APIs are subject to frequent breaking changes.

## Current Next Steps

A rough plan of the next items to get working is:

1. ~~Typecheck `int` vs `bool` expressions for basic operations.~~
2. ~~Emit C code based on IL instead of semantic tree~~
3. Checking binding mutability (`let` vs `var`) on `int` and `bool` variable types.
4. Typecheck strings to handle overload of `+`?
5. Optional types?
6. ~~Add classes without members.~~
7. Borrow checking on reference types.
8. Destructors (Give a talk at the Rust meetup on drop order in Rust vs Adamant)s
9. Interfaces
10. Overload functions on # params
11. Overload functions on types

## Cleanup Tasks

* Remove the deprecated `Match` and `MatchInto` classes.

## Conventions

* Unit tests are in projects named `Tests.Unit.*`. This way, it is not inconsistent when further namespaces are nested inside them. If `Tests` were at the end of the name, then many namespaces would have it at the end, while nested ones would have it in the middle. This also allows conformance and integration tests to be grouped with them by placing them all under the `Tests` namespace.
* Reshaper nullability attributes are used to help find issues with null handling. Occasionally, the inference is incorrect because framework calls aren't correctly annotated. In such cases, the `AssertNotNull()` extension method is used. All methods taking non-null arguments should use `Requires.NotNull()` to enforce this. This is not necessary on values passed only to the base class constructor as the base constructor is responsible for checking. This null handling helps to support correct by construction and to flag errors as close to their source as possible.
* Namespace hierarchies are kept fairly flat. This is to avoid issues of needing too many `using` statements and moving types between them. A namespace should contain all classes that represent the same "kind" of entity without much regard to sub-kinds. Originally, this was not the case. Types were separated into many sub-namespaces, but this ended up being more trouble than it was worth. In practice, one still just used the go to type functionality to find types.
