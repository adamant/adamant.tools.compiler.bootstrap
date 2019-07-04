#include "RuntimeLibrary.h"
#include <stdio.h>

// Reminder: `extern` function declarations are so the compiler knows what
// object file to put the non-inline copies of inlined functions in.

// `bool` type
extern inline _bool _bool__and(_bool x, _bool y);
extern inline _bool _bool__or(_bool x, _bool y);

// Extern Integer Type Operations Macro
#define EXTERN_INTEGER_OPERATIONS(type) \
extern inline type type##__add(type x, type y); \
extern inline type type##__sub(type x, type y); \
extern inline type type##__mul(type x, type y); \
extern inline type type##__div(type x, type y); \
extern inline type type##__remainder__1(type x, type y); \
extern inline _bool type##__eq(type x, type y); \
extern inline _bool type##__ne(type x, type y); \
extern inline _bool type##__lt(type x, type y); \
extern inline _bool type##__lte(type x, type y); \
extern inline _bool type##__gt(type x, type y); \
extern inline _bool type##__gte(type x, type y);

// `int` type
EXTERN_INTEGER_OPERATIONS(_int)

// `uint` type
EXTERN_INTEGER_OPERATIONS(_uint)

// `byte` type
EXTERN_INTEGER_OPERATIONS(_byte)

// `size` type
EXTERN_INTEGER_OPERATIONS(_size)

// `offset` type
EXTERN_INTEGER_OPERATIONS(_offset)

// `String` type
extern String String___op_string_literal__2(_size count, _byte* bytes);

// Direct support for console IO through the runtime for now
void print_string__1(String text)
{
    printf("%.*s", (int)text.byte_count._value, (char*)text.bytes);
}

String _uint__to_display_string__0(_uint value)
{
    int length = snprintf(NULL, 0, "%u", value._value);
    char* str = malloc(length + 1);
    snprintf(str, length + 1, "%u", value._value);
    return (String) { (_size) { length }, (_byte*)str };
}

// Test of calling windows memory allocation functions, rather than including
// windows.h, we directly declare the functions. This demonstrates that external
// function calls could do this.
//#include <windows.h>
typedef void* LPVOID;
typedef LPVOID HANDLE;
typedef size_t SIZE_T;
typedef uint32_t DWORD;
typedef int32_t BOOL;

HANDLE GetProcessHeap();
LPVOID HeapAlloc(HANDLE hHeap, DWORD dwFlags, SIZE_T dwBytes);
LPVOID HeapReAlloc(HANDLE hHeap, DWORD dwFlags, LPVOID lpMem, SIZE_T dwBytes);
BOOL HeapFree(HANDLE hHeap, DWORD dwFlags, LPVOID lpMem);
DWORD GetLastError();

void test()
{
    void* ptr = HeapAlloc(GetProcessHeap(), 0, 10);
    HeapFree(GetProcessHeap(), 0, ptr);
    //void* ptr = aligned_alloc(8, 32);
    //free(ptr);
}
