#include <stdint.h>

// Rather than including all of stdlib.h we only declare the things we need.
void* malloc(size_t size);
void free(void* ptr);

// Cross platform way to mark a variable or parameter as unused to avoid warnings
#define _UNUSED(x) (void)(x)

#define OPTIONAL_TYPE(type) \
typedef struct { _Bool _hasValue; type _value; } _opt__##type; \
inline _opt__##type _opt__##type##__Some(type x) { return (_opt__##type) {1, x}; } \
extern const _opt__##type _opt__##type##__none;

// `bool` type
typedef struct { _Bool _value; } _bool;

inline _bool _bool__and(_bool x, _bool y) { return (_bool) { x._value& y._value }; }
inline _bool _bool__or(_bool x, _bool y) { return (_bool) { x._value | y._value }; }

OPTIONAL_TYPE(_bool)

// Integer Type Operations Macro
#define INTEGER_OPERATIONS(type) \
inline type type##__add(type x, type y) { return (type) { x._value + y._value }; } \
inline type type##__sub(type x, type y) { return (type) { x._value - y._value }; } \
inline type type##__mul(type x, type y) { return (type) { x._value* y._value }; } \
inline type type##__div(type x, type y) { return (type) { x._value / y._value }; } \
inline type type##__remainder__2(type x, type y) { return (type) { x._value% y._value }; } \
inline _bool type##__eq(type x, type y) { return (_bool) { x._value == y._value }; } \
inline _bool type##__ne(type x, type y) { return (_bool) { x._value != y._value }; } \
inline _bool type##__lt(type x, type y) { return (_bool) { x._value < y._value }; } \
inline _bool type##__lte(type x, type y) { return (_bool) { x._value <= y._value }; } \
inline _bool type##__gt(type x, type y) { return (_bool) { x._value > y._value }; } \
inline _bool type##__gte(type x, type y) { return (_bool) { x._value >= y._value }; }

// `int` type
typedef struct { int32_t _value; } _int;

INTEGER_OPERATIONS(_int)
OPTIONAL_TYPE(_int)

// `uint` type
typedef struct { uint32_t _value; } _uint;

INTEGER_OPERATIONS(_uint)

// `byte` type
typedef struct { uint8_t _value; } _byte;

INTEGER_OPERATIONS(_byte)

// `size` type
typedef struct { uintptr_t _value; } _size;

INTEGER_OPERATIONS(_size)

// `offset` type
typedef struct { intptr_t _value; } _offset;

INTEGER_OPERATIONS(_offset)

// conversions
inline _int _convert___byte___int(_byte value) { return (_int) {value._value}; }

// Forward declare String that is in std lib for use by intrinsics
typedef struct String String;

// Intrinsic Functions
String _uint__to_display_string__1(_uint value);
_size intrinsics__mem_allocate__1(_size length);
void intrinsics__mem_deallocate__1(_size ptr);
void intrinsics__mem_copy__3(_size from_ptr, _size to_ptr, _size length);
inline void intrinsics__mem_set_byte__2(_size ptr, _byte byte)
{
    *((uint8_t*)ptr._value) = byte._value;
}
inline _byte intrinsics__mem_get_byte__1(_size ptr)
{
  return (_byte) { *((uint8_t*)ptr._value) };
}
void intrinsics__print_utf8__2(_size ptr, _size length);
_size intrinsics__read_utf8_line__2(_size ptr, _size length);
