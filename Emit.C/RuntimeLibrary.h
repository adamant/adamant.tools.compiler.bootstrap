#include <stdint.h>

// Rather than including all of stdlib.h we only declare the one thing we need.
void* malloc(size_t size);

#define _UNUSED(x) (void)(x)

// `bool` type
typedef struct { _Bool _value; } _bool;

inline _bool _bool__and(_bool x, _bool y) { return (_bool){ x._value & y._value }; }
inline _bool _bool__or(_bool x, _bool y) { return (_bool){ x._value | y._value }; }

// `int` type
typedef struct { int32_t _value; } _int;

inline _int _int__add(_int x, _int y) { return (_int){ x._value + y._value }; }
inline _int _int__sub(_int x, _int y) { return (_int){ x._value - y._value }; }
inline _int _int__mul(_int x, _int y) { return (_int){ x._value * y._value }; }
inline _int _int__div(_int x, _int y) { return (_int){ x._value / y._value }; }
inline _int _int__remainder__1(_int x, _int y) { return (_int){ x._value % y._value }; }

inline _bool _int__eq(_int x, _int y) { return (_bool){ x._value == y._value }; }
inline _bool _int__ne(_int x, _int y) { return (_bool){ x._value != y._value }; }
inline _bool _int__lt(_int x, _int y) { return (_bool){ x._value < y._value }; }
inline _bool _int__lte(_int x, _int y) { return (_bool){ x._value <= y._value }; }
inline _bool _int__gt(_int x, _int y) { return (_bool){ x._value > y._value }; }
inline _bool _int__gte(_int x, _int y) { return (_bool){ x._value >= y._value }; }

// `uint` type
typedef struct { uint32_t _value; } _uint;

inline _uint _uint__add(_uint x, _uint y) { return (_uint){ x._value + y._value }; }
inline _uint _uint__sub(_uint x, _uint y) { return (_uint){ x._value - y._value }; }
inline _uint _uint__mul(_uint x, _uint y) { return (_uint){ x._value * y._value }; }
inline _uint _uint__div(_uint x, _uint y) { return (_uint){ x._value / y._value }; }
inline _uint _uint__remainder__1(_uint x, _uint y) { return (_uint){ x._value % y._value }; }

inline _bool _uint__eq(_uint x, _uint y) { return (_bool){ x._value == y._value }; }
inline _bool _uint__ne(_uint x, _uint y) { return (_bool){ x._value != y._value }; }
inline _bool _uint__lt(_uint x, _uint y) { return (_bool){ x._value < y._value }; }
inline _bool _uint__lte(_uint x, _uint y) { return (_bool){ x._value <= y._value }; }
inline _bool _uint__gt(_uint x, _uint y) { return (_bool){ x._value > y._value }; }
inline _bool _uint__gte(_uint x, _uint y) { return (_bool){ x._value >= y._value }; }



// Other integer types
typedef struct { uint8_t _value; } _byte;
typedef struct { uintptr_t _value; } _size;
typedef struct { intptr_t _value; } _offset;

// `String` type
// Note: For now, we have moved strings
typedef struct {
    _size byte_count;
    _byte* bytes;
} String;

inline String String___op_string_literal__2(_size byte_count, _byte* bytes)
{
    return (String){ byte_count, bytes };
}

// Direct support through the runtime for now
void print_string__1(String text);
String _uint__to_display_string__0(_uint value);
