# Core Features

This is a list of core features that need to be implemented or at least accounted for in the design of the compiler.

* Borrow Checking (lifetimes)
* Rebinding of local variables (for `let` and in separate scopes for `var`)
* Binding mutability checking (i.e. `let` vs `var`)
  * Local variables
  * Parameters
  * Self Parameter
  * Global Variables
* Mutability
  * Reference Types (on class and type declaration)
  * Value Types
* Type Checking
* Overload Functions on Arity
* Overload Functions on Type
* Runtime Code Execution
* Generic Types
* Associated Functions
* Generic Types over Values (i.e. fixed length arrays)
* Definite assignment
* Destructor value access rules
* Exception types
* Effect Typing
* Async/await
* Inheritance
* Interface implementation
* Closures (esp. given lifetimes)
* Pseudo references
* Optional Types
* Void type as type parameter
* Unsafe code
* Type Inference
* Constant Evaluation
