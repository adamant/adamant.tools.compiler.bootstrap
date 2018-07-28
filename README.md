# Adamant Bootstrap Compiler

A bootstrap compiler for the [Adamant language](http://adamant-lang.org).  That is, a compiler for a subset of the language that will be used to write the Adamant compiler in Adamant. The compiler currently compiles a small subset of the Adamant language to C.

## Project Status: Alpha Active

The compiler is under active development. It is in a very early stage, and there are likely issues and limitations. APIs are subject to frequent breaking changes.

## Current Next Steps

A rough plan of the next items to get working is:

1. Typecheck `int` vs `bool` expressions for basic operations.
2. Checking binding mutability (`let` vs `var`) on `int` and `bool` variable types.
3. Typecheck strings to handle overload of `+`?
4. Optional types?
5. Add classes without members.
6. Borrow checking on reference types.
7. Destructors (Give a talk at the Rust meetup on drop order in Rust vs Adamant)s
8. Interfaces
9. Overload functions on # params
10. Overload functions on types
