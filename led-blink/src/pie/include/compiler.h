#pragma once
#define PIE_VERIFY_STATIC static_assert

//--------------------------------------------------------------------+
// Compiler porting with Attribute and Endian
//--------------------------------------------------------------------+
#if defined(__GNUC__)
#define PIE_ATTR_ALIGNED(bytes) __attribute__((aligned(bytes)))
#define PIE_ATTR_PACKED __attribute__((packed))
#define PIE_ATTR_UNUSED __attribute__((unused))
#define PIE_ATTR_USED __attribute__((used))
#define PIE_INLINE inline
#define PIE_STATIC_INLINE static inline

#define PIE_LITTLE_ENDIAN (0x12u)
#define PIE_BIG_ENDIAN (0x21u)

// Compile-time Assert
#if defined (__STDC_VERSION__) && __STDC_VERSION__ >= 201112L
  #define PIE_VERIFY_STATIC   _Static_assert
#elif defined (__cplusplus) && __cplusplus >= 201103L
  #define PIE_VERIFY_STATIC   static_assert
#else
  #define PIE_VERIFY_STATIC(const_expr, _mess) enum { TU_XSTRCAT(_verify_static_, _TU_COUNTER_) = 1/(!!(const_expr)) }
#endif

// Endian conversion use well-known host to network (big endian) naming
#if __BYTE_ORDER__ == __ORDER_LITTLE_ENDIAN__
#define PIE_BYTE_ORDER PIE_LITTLE_ENDIAN
#define U32_LOBYTE(x) ((uint16_t)(((uint32_t)x) & 0xFFFF))
#define U32_HIBYTE(x) ((uint16_t)(((uint32_t)x) & 0xFFFF0000) >> 16)
#define U16_LOBYTE(x) ((uint8_t)(((uint16_t)x) & 0x00FF))
#define U16_HIBYTE(x) ((uint8_t)((((uint16_t)x) & 0xFF00) >> 8))
#else
#define PIE_BYTE_ORDER PIE_BIG_ENDIAN
#define U32_LOBYTE(x) ((uint16_t)(((uint32_t)x) & 0xFFFF0000) >> 16)
#define U32_HIBYTE(x) ((uint16_t)(((uint32_t)x) & 0xFFFF))
#define U16_LOBYTE(x) ((uint8_t)((((uint16_t)x) & 0xFF00) >> 8))
#define U16_HIBYTE(x) ((uint8_t)(((uint16_t)x) & 0x00FF))
#endif

#define PIE_BSWAP16(u16) (__builtin_bswap16(u16))
#define PIE_BSWAP32(u32) (__builtin_bswap32(u32))
// endif __GNUC__
#else
#error "Compiler attribute porting is required"
#endif