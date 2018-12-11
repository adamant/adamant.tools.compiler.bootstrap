#include "RuntimeLibrary.h"
#include <stdio.h>

// Reminder: `extern` function declarations are so the compiler knows what
// object file to put the non-inline copies of inlined functions in.

// `bool` type
extern inline _bool _bool__and(_bool x, _bool y);
extern inline _bool _bool__or(_bool x, _bool y);

// `int` type
extern inline _int _int__add(_int x, _int y);
extern inline _int _int__sub(_int x, _int y);
extern inline _int _int__mul(_int x, _int y);
extern inline _int _int__div(_int x, _int y);
extern inline _int _int__remainder__1(_int x, _int y);

extern inline _bool _int__eq(_int x, _int y);
extern inline _bool _int__ne(_int x, _int y);
extern inline _bool _int__lt(_int x, _int y);
extern inline _bool _int__lte(_int x, _int y);
extern inline _bool _int__gt(_int x, _int y);
extern inline _bool _int__gte(_int x, _int y);

// `uint` type
extern inline _uint _uint__add(_uint x, _uint y);
extern inline _uint _uint__sub(_uint x, _uint y);
extern inline _uint _uint__mul(_uint x, _uint y);
extern inline _uint _uint__div(_uint x, _uint y);
extern inline _uint _uint__remainder__1(_uint x, _uint y);

extern inline _bool _uint__eq(_uint x, _uint y);
extern inline _bool _uint__ne(_uint x, _uint y);
extern inline _bool _uint__lt(_uint x, _uint y);
extern inline _bool _uint__lte(_uint x, _uint y);
extern inline _bool _uint__gt(_uint x, _uint y);
extern inline _bool _uint__gte(_uint x, _uint y);

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
    char* str = malloc(length+1);
    snprintf(str, length + 1, "%u", value._value);
    return (String){(_size){length}, (_byte*)str};
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
