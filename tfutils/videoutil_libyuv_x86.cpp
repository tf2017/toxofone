#include <stdint.h>
#include <string.h>
#include <intrin.h>  // For __cpuidex()
#include <tmmintrin.h>  // For _mm_maddubs_epi16
#include <emmintrin.h>

/*
This code was cherrypicked from LibYUV, https://chromium.googlesource.com/libyuv/libyuv/
*/

#define IS_ALIGNED(p, a) (!((uintptr_t)(p) & ((a) - 1)))

#define SIMD_ALIGNED(var) __declspec(align(16)) var
#define SIMD_ALIGNED32(var) __declspec(align(64)) var
typedef __declspec(align(16)) int8_t vec8[16];
typedef __declspec(align(16)) int16_t vec16[8];
typedef __declspec(align(16)) int32_t vec32[4];
typedef __declspec(align(16)) uint8_t uvec8[16];
typedef __declspec(align(16)) uint16_t uvec16[8];
typedef __declspec(align(16)) uint32_t uvec32[4];
typedef __declspec(align(32)) int8_t lvec8[32];
typedef __declspec(align(32)) int16_t lvec16[16];
typedef __declspec(align(32)) int32_t lvec32[8];
typedef __declspec(align(32)) uint8_t ulvec8[32];
typedef __declspec(align(32)) uint16_t ulvec16[16];
typedef __declspec(align(32)) uint32_t ulvec32[8];

// This struct is for Intel color conversion.
struct YuvConstants
{
    lvec8 kUVToB;
    lvec8 kUVToG;
    lvec8 kUVToR;
    lvec16 kUVBiasB;
    lvec16 kUVBiasG;
    lvec16 kUVBiasR;
    lvec16 kYToRgb;
};

// Offsets into YuvConstants structure
#define KUVTOB   0
#define KUVTOG   32
#define KUVTOR   64
#define KUVBIASB 96
#define KUVBIASG 128
#define KUVBIASR 160
#define KYTORGB  192

// These flags are only valid on x86 processors.
static const int kCpuHasX86 = 0x10;
static const int kCpuHasSSSE3 = 0x40;

static void CpuId(uint32_t info_eax, uint32_t info_ecx, uint32_t* cpu_info)
{
    // Visual C version uses intrinsic or inline x86 assembly.
#if (_MSC_FULL_VER >= 160040219)
    __cpuidex((int32_t*)(cpu_info), info_eax, info_ecx);
#elif defined(_M_IX86)
    __asm
    {
        mov        eax, info_eax
        mov        ecx, info_ecx
        mov        edi, cpu_info
        cpuid
        mov[edi], eax
        mov[edi + 4], ebx
        mov[edi + 8], ecx
        mov[edi + 12], edx
    }
#else  // Visual C but not x86
    if (info_ecx == 0)
    {
        __cpuid((int*)(cpu_info), info_eax);
    }
    else
    {
        cpu_info[3] = cpu_info[2] = cpu_info[1] = cpu_info[0] = 0;
    }
#endif
}

static int CpuFlags(void)
{
    uint32_t cpu_info0[4] = { 0, 0, 0, 0 };
    uint32_t cpu_info1[4] = { 0, 0, 0, 0 };
    int cpu_info = 0;

    CpuId(0, 0, cpu_info0);
    CpuId(1, 0, cpu_info1);
    cpu_info = ((cpu_info1[2] & 0x00000200) ? kCpuHasSSSE3 : 0) | kCpuHasX86;

    return cpu_info;
}

// Detect CPU has SSE2 etc.
// Test_flag parameter should be one of kCpuHas constants above.
// returns non-zero if instruction set is detected
static __inline int TestCpuFlag(int test_flag)
{
    return CpuFlags() & test_flag;
}

static __inline int RGBToY(uint8_t r, uint8_t g, uint8_t b) {
    return (66 * r + 129 * g + 25 * b + 0x1080) >> 8;
}

static __inline int RGBToU(uint8_t r, uint8_t g, uint8_t b) {
    return (112 * b - 74 * g - 38 * r + 0x8080) >> 8;
}
static __inline int RGBToV(uint8_t r, uint8_t g, uint8_t b) {
    return (112 * r - 94 * g - 18 * b + 0x8080) >> 8;
}

#define MAKEROWY(NAME, R, G, B, BPP) \
void NAME ## ToYRow_C(const uint8_t* src_argb0, uint8_t* dst_y, int32_t width) \
{                                                                              \
  int32_t x;                                                                   \
  for (x = 0; x < width; ++x)                                                  \
  {                                                                            \
    dst_y[0] = RGBToY(src_argb0[R], src_argb0[G], src_argb0[B]);               \
    src_argb0 += BPP;                                                          \
    dst_y += 1;                                                                \
  }                                                                            \
}                                                                              \
void NAME ## ToUVRow_C(const uint8_t* src_rgb0, int32_t src_stride_rgb,        \
                       uint8_t* dst_u, uint8_t* dst_v, int32_t width)          \
{                                                                              \
  const uint8_t* src_rgb1 = src_rgb0 + src_stride_rgb;                         \
  int32_t x;                                                                   \
  for (x = 0; x < width - 1; x += 2)                                           \
  {                                                                            \
    uint8_t ab = (src_rgb0[B] + src_rgb0[B + BPP] +                            \
                  src_rgb1[B] + src_rgb1[B + BPP]) >> 2;                       \
    uint8_t ag = (src_rgb0[G] + src_rgb0[G + BPP] +                            \
                  src_rgb1[G] + src_rgb1[G + BPP]) >> 2;                       \
    uint8_t ar = (src_rgb0[R] + src_rgb0[R + BPP] +                            \
                  src_rgb1[R] + src_rgb1[R + BPP]) >> 2;                       \
    dst_u[0] = RGBToU(ar, ag, ab);                                             \
    dst_v[0] = RGBToV(ar, ag, ab);                                             \
    src_rgb0 += BPP * 2;                                                       \
    src_rgb1 += BPP * 2;                                                       \
    dst_u += 1;                                                                \
    dst_v += 1;                                                                \
  }                                                                            \
  if (width & 1)                                                               \
  {                                                                            \
    uint8_t ab = (src_rgb0[B] + src_rgb1[B]) >> 1;                             \
    uint8_t ag = (src_rgb0[G] + src_rgb1[G]) >> 1;                             \
    uint8_t ar = (src_rgb0[R] + src_rgb1[R]) >> 1;                             \
    dst_u[0] = RGBToU(ar, ag, ab);                                             \
    dst_v[0] = RGBToV(ar, ag, ab);                                             \
  }                                                                            \
}

MAKEROWY(ARGB, 2, 1, 0, 4)
#undef MAKEROWY

// Constants for ARGB.
static const vec8 kARGBToY =
{
    13, 65, 33, 0, 13, 65, 33, 0, 13, 65, 33, 0, 13, 65, 33, 0
};

static const uvec8 kAddY16 =
{
    16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u, 16u
};

// Convert 16 ARGB pixels (64 bytes) to 16 Y values.
__declspec(naked)
void ARGBToYRow_SSSE3(const uint8_t* src_argb, uint8_t* dst_y, int32_t width) {
    __asm
    {
        mov        eax, [esp + 4]   /* src_argb */
        mov        edx, [esp + 8]   /* dst_y */
        mov        ecx, [esp + 12]  /* width */
        movdqa     xmm4, xmmword ptr kARGBToY
        movdqa     xmm5, xmmword ptr kAddY16

        convertloop :
        movdqu     xmm0, [eax]
        movdqu     xmm1, [eax + 16]
        movdqu     xmm2, [eax + 32]
        movdqu     xmm3, [eax + 48]
        pmaddubsw  xmm0, xmm4
        pmaddubsw  xmm1, xmm4
        pmaddubsw  xmm2, xmm4
        pmaddubsw  xmm3, xmm4
        lea        eax, [eax + 64]
        phaddw     xmm0, xmm1
        phaddw     xmm2, xmm3
        psrlw      xmm0, 7
        psrlw      xmm2, 7
        packuswb   xmm0, xmm2
        paddb      xmm0, xmm5
        movdqu[edx], xmm0
        lea        edx, [edx + 16]
        sub        ecx, 16
        jg         convertloop
        ret
    }
}

static const vec8 kARGBToU =
{
    112, -74, -38, 0, 112, -74, -38, 0, 112, -74, -38, 0, 112, -74, -38, 0
};

static const vec8 kARGBToV =
{
    -18, -94, 112, 0, -18, -94, 112, 0, -18, -94, 112, 0, -18, -94, 112, 0,
};

static const uvec8 kAddUV128 =
{
    128u, 128u, 128u, 128u, 128u, 128u, 128u, 128u,
    128u, 128u, 128u, 128u, 128u, 128u, 128u, 128u
};

__declspec(naked)
void ARGBToUVRow_SSSE3(const uint8_t* src_argb0, int32_t src_stride_argb,
    uint8_t* dst_u, uint8_t* dst_v, int32_t width)
{
    __asm
    {
        push       esi
        push       edi
        mov        eax, [esp + 8 + 4]   // src_argb
        mov        esi, [esp + 8 + 8]   // src_stride_argb
        mov        edx, [esp + 8 + 12]  // dst_u
        mov        edi, [esp + 8 + 16]  // dst_v
        mov        ecx, [esp + 8 + 20]  // width
        movdqa     xmm5, xmmword ptr kAddUV128
        movdqa     xmm6, xmmword ptr kARGBToV
        movdqa     xmm7, xmmword ptr kARGBToU
        sub        edi, edx             // stride from u to v

        convertloop :
        /* step 1 - subsample 16x2 argb pixels to 8x1 */
        movdqu     xmm0, [eax]
        movdqu     xmm4, [eax + esi]
        pavgb      xmm0, xmm4
        movdqu     xmm1, [eax + 16]
        movdqu     xmm4, [eax + esi + 16]
        pavgb      xmm1, xmm4
        movdqu     xmm2, [eax + 32]
        movdqu     xmm4, [eax + esi + 32]
        pavgb      xmm2, xmm4
        movdqu     xmm3, [eax + 48]
        movdqu     xmm4, [eax + esi + 48]
        pavgb      xmm3, xmm4

        lea        eax, [eax + 64]
        movdqa     xmm4, xmm0
        shufps     xmm0, xmm1, 0x88
        shufps     xmm4, xmm1, 0xdd
        pavgb      xmm0, xmm4
        movdqa     xmm4, xmm2
        shufps     xmm2, xmm3, 0x88
        shufps     xmm4, xmm3, 0xdd
        pavgb      xmm2, xmm4

        // step 2 - convert to U and V
        // from here down is very similar to Y code except
        // instead of 16 different pixels, its 8 pixels of U and 8 of V
        movdqa     xmm1, xmm0
        movdqa     xmm3, xmm2
        pmaddubsw  xmm0, xmm7  // U
        pmaddubsw  xmm2, xmm7
        pmaddubsw  xmm1, xmm6  // V
        pmaddubsw  xmm3, xmm6
        phaddw     xmm0, xmm2
        phaddw     xmm1, xmm3
        psraw      xmm0, 8
        psraw      xmm1, 8
        packsswb   xmm0, xmm1
        paddb      xmm0, xmm5  // -> unsigned

        // step 3 - store 8 U and 8 V values
        movlps     qword ptr[edx], xmm0 // U
        movhps     qword ptr[edx + edi], xmm0 // V
        lea        edx, [edx + 8]
        sub        ecx, 16
        jg         convertloop

        pop        edi
        pop        esi
        ret
    }
}

// Subsampled source needs to be increase by 1 of not even.
#define SS(width, shift) (((width) + (1 << (shift)) - 1) >> (shift))

// Any 1 to 1.
#define ANY11(NAMEANY, ANY_SIMD, UVSHIFT, SBPP, BPP, MASK)                     \
    void NAMEANY(const uint8_t* src_ptr, uint8_t* dst_ptr, int32_t width)      \
    {                                                                          \
      SIMD_ALIGNED(uint8_t temp[128 * 2]);                                     \
      memset(temp, 0, 128);  /* for YUY2 and msan */                           \
      int32_t r = width & MASK;                                                \
      int32_t n = width & ~MASK;                                               \
      if (n > 0)                                                               \
      {                                                                        \
        ANY_SIMD(src_ptr, dst_ptr, n);                                         \
      }                                                                        \
      memcpy(temp, src_ptr + (n >> UVSHIFT) * SBPP, SS(r, UVSHIFT) * SBPP);    \
      ANY_SIMD(temp, temp + 128, MASK + 1);                                    \
      memcpy(dst_ptr + n * BPP, temp + 128, r * BPP);                          \
    }

// Any 1 to 2 with source stride (2 rows of source).  Outputs UV planes.
// 128 byte row allows for 32 avx ARGB pixels.
#define ANY12S(NAMEANY, ANY_SIMD, UVSHIFT, BPP, MASK)                          \
    void NAMEANY(const uint8_t* src_ptr, int32_t src_stride_ptr,               \
                 uint8_t* dst_u, uint8_t* dst_v, int32_t width)                \
    {                                                                          \
      SIMD_ALIGNED(uint8_t temp[128 * 4]);                                     \
      memset(temp, 0, 128 * 2);  /* for msan */                                \
      int32_t r = width & MASK;                                                \
      int32_t n = width & ~MASK;                                               \
      if (n > 0)                                                               \
      {                                                                        \
        ANY_SIMD(src_ptr, src_stride_ptr, dst_u, dst_v, n);                    \
      }                                                                        \
      memcpy(temp, src_ptr  + (n >> UVSHIFT) * BPP, SS(r, UVSHIFT) * BPP);     \
      memcpy(temp + 128, src_ptr  + src_stride_ptr + (n >> UVSHIFT) * BPP, SS(r, UVSHIFT) * BPP); \
      if ((width & 1) && UVSHIFT == 0)                                         \
      {  /* repeat last pixel for subsample */                                 \
        memcpy(temp + SS(r, UVSHIFT) * BPP,                                    \
               temp + SS(r, UVSHIFT) * BPP - BPP, BPP);                        \
        memcpy(temp + 128 + SS(r, UVSHIFT) * BPP,                              \
               temp + 128 + SS(r, UVSHIFT) * BPP - BPP, BPP);                  \
      }                                                                        \
      ANY_SIMD(temp, 128, temp + 256, temp + 384, MASK + 1);                   \
      memcpy(dst_u + (n >> 1), temp + 256, SS(r, 1));                          \
      memcpy(dst_v + (n >> 1), temp + 384, SS(r, 1));                          \
    }

// Note that odd width replication includes 444 due to implementation
// on arm that subsamples 444 to 422 internally.
// Any 3 planes to 1 with yuvconstants
#define ANY31C(NAMEANY, ANY_SIMD, UVSHIFT, DUVSHIFT, BPP, MASK)                \
    void NAMEANY(const uint8_t* y_buf, const uint8_t* u_buf, const uint8_t* v_buf, \
                 uint8_t* dst_ptr, const struct YuvConstants* yuvconstants,    \
                 int32_t width)                                                \
    {                                                                          \
      SIMD_ALIGNED(uint8_t temp[64 * 4]);                                      \
      memset(temp, 0, 64 * 3);  /* for YUY2 and msan */                        \
      int32_t r = width & MASK;                                                \
      int32_t n = width & ~MASK;                                               \
      if (n > 0)                                                               \
      {                                                                        \
        ANY_SIMD(y_buf, u_buf, v_buf, dst_ptr, yuvconstants, n);               \
      }                                                                        \
      memcpy(temp, y_buf + n, r);                                              \
      memcpy(temp + 64, u_buf + (n >> UVSHIFT), SS(r, UVSHIFT));               \
      memcpy(temp + 128, v_buf + (n >> UVSHIFT), SS(r, UVSHIFT));              \
      if (width & 1)                                                           \
      {                                                                        \
        temp[64 + SS(r, UVSHIFT)] = temp[64 + SS(r, UVSHIFT) - 1];             \
        temp[128 + SS(r, UVSHIFT)] = temp[128 + SS(r, UVSHIFT) - 1];           \
      }                                                                        \
      ANY_SIMD(temp, temp + 64, temp + 128, temp + 192, yuvconstants, MASK + 1);  \
      memcpy(dst_ptr + (n >> DUVSHIFT) * BPP, temp + 192, SS(r, DUVSHIFT) * BPP); \
    }

ANY11(ARGBToYRow_Any_SSSE3, ARGBToYRow_SSSE3, 0, 4, 1, 15)
ANY12S(ARGBToUVRow_Any_SSSE3, ARGBToUVRow_SSSE3, 0, 4, 15)

static __inline int32_t clamp0(int32_t v)
{
    return ((-(v) >> 31) & (v));
}

static __inline int32_t clamp255(int32_t v)
{
    return (((255 - (v)) >> 31) | (v)) & 255;
}

static __inline uint32_t Clamp(int32_t val)
{
    int v = clamp0(val);
    return (uint32_t)(clamp255(v));
}

static __inline uint32_t Abs(int32_t v)
{
    int m = v >> 31;
    return (v + m) ^ m;
}

// C reference code that mimics the YUV assembly.
static __inline void YuvPixel(uint8_t y, uint8_t u, uint8_t v,
    uint8_t* b, uint8_t* g, uint8_t* r,
    const struct YuvConstants* yuvconstants)
{
    int ub = yuvconstants->kUVToB[0];
    int ug = yuvconstants->kUVToG[0];
    int vg = yuvconstants->kUVToG[1];
    int vr = yuvconstants->kUVToR[1];
    int bb = yuvconstants->kUVBiasB[0];
    int bg = yuvconstants->kUVBiasG[0];
    int br = yuvconstants->kUVBiasR[0];
    int yg = yuvconstants->kYToRgb[0];

    uint32_t y1 = (uint32_t)(y * 0x0101 * yg) >> 16;
    *b = Clamp((int32_t)(-(u * ub) + y1 + bb) >> 6);
    *g = Clamp((int32_t)(-(u * ug + v * vg) + y1 + bg) >> 6);
    *r = Clamp((int32_t)(-(v * vr) + y1 + br) >> 6);
}

// BT.601 YUV to RGB reference
//  R = (Y - 16) * 1.164              - V * -1.596
//  G = (Y - 16) * 1.164 - U *  0.391 - V *  0.813
//  B = (Y - 16) * 1.164 - U * -2.018

// Y contribution to R,G,B.  Scale and bias.
#define YG (18997) /* round(1.164 * 64 * 256 * 256 / 257) */
#define YGB (-1160) /* 1.164 * 64 * -16 + 64 / 2 */

// U and V contributions to R,G,B.
#define UB (-128) /* max(-128, round(-2.018 * 64)) */
#define UG (25) /* round(0.391 * 64) */
#define VG (52) /* round(0.813 * 64) */
#define VR (-102) /* round(-1.596 * 64) */

// Bias values to subtract 16 from Y and 128 from U and V.
#define BB (UB * 128            + YGB)
#define BG (UG * 128 + VG * 128 + YGB)
#define BR            (VR * 128 + YGB)

const YuvConstants SIMD_ALIGNED(kYuvI601Constants) =
{
    { UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0, UB, 0 },
    { UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG, UG, VG },
    { 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR, 0, VR },
    { BB, BB, BB, BB, BB, BB, BB, BB, BB, BB, BB, BB, BB, BB, BB, BB },
    { BG, BG, BG, BG, BG, BG, BG, BG, BG, BG, BG, BG, BG, BG, BG, BG },
    { BR, BR, BR, BR, BR, BR, BR, BR, BR, BR, BR, BR, BR, BR, BR, BR },
    { YG, YG, YG, YG, YG, YG, YG, YG, YG, YG, YG, YG, YG, YG, YG, YG }
};

// Also used for 420
static void I422ToARGBRow_C(const uint8_t* src_y,
    const uint8_t* src_u,
    const uint8_t* src_v,
    uint8_t* rgb_buf,
    const struct YuvConstants* yuvconstants,
    int32_t width)
{
    int32_t x;
    for (x = 0; x < width - 1; x += 2)
    {
        YuvPixel(src_y[0], src_u[0], src_v[0], rgb_buf + 0, rgb_buf + 1, rgb_buf + 2, yuvconstants);
        rgb_buf[3] = 255;
        YuvPixel(src_y[1], src_u[0], src_v[0], rgb_buf + 4, rgb_buf + 5, rgb_buf + 6, yuvconstants);
        rgb_buf[7] = 255;
        src_y += 2;
        src_u += 1;
        src_v += 1;
        rgb_buf += 8;  // Advance 2 pixels.
    }
    if (width & 1)
    {
        YuvPixel(src_y[0], src_u[0], src_v[0], rgb_buf + 0, rgb_buf + 1, rgb_buf + 2, yuvconstants);
        rgb_buf[3] = 255;
    }
}

// Read 4 UV from 422, upsample to 8 UV.
#define READYUV422 __asm {                                                     \
    __asm movd       xmm0, [esi]          /* U */                              \
    __asm movd       xmm1, [esi + edi]    /* V */                              \
    __asm lea        esi,  [esi + 4]                                           \
    __asm punpcklbw  xmm0, xmm1           /* UV */                             \
    __asm punpcklwd  xmm0, xmm0           /* UVUV (upsample) */                \
    __asm movq       xmm4, qword ptr [eax]                                     \
    __asm punpcklbw  xmm4, xmm4                                                \
    __asm lea        eax, [eax + 8]                                            \
  }

// Convert 8 pixels: 8 UV and 8 Y.
#define YUVTORGB(YuvConstants) __asm {                                         \
    __asm movdqa     xmm1, xmm0                                                \
    __asm movdqa     xmm2, xmm0                                                \
    __asm movdqa     xmm3, xmm0                                                \
    __asm movdqa     xmm0, xmmword ptr [YuvConstants + KUVBIASB]               \
    __asm pmaddubsw  xmm1, xmmword ptr [YuvConstants + KUVTOB]                 \
    __asm psubw      xmm0, xmm1                                                \
    __asm movdqa     xmm1, xmmword ptr [YuvConstants + KUVBIASG]               \
    __asm pmaddubsw  xmm2, xmmword ptr [YuvConstants + KUVTOG]                 \
    __asm psubw      xmm1, xmm2                                                \
    __asm movdqa     xmm2, xmmword ptr [YuvConstants + KUVBIASR]               \
    __asm pmaddubsw  xmm3, xmmword ptr [YuvConstants + KUVTOR]                 \
    __asm psubw      xmm2, xmm3                                                \
    __asm pmulhuw    xmm4, xmmword ptr [YuvConstants + KYTORGB]                \
    __asm paddsw     xmm0, xmm4           /* B += Y */                         \
    __asm paddsw     xmm1, xmm4           /* G += Y */                         \
    __asm paddsw     xmm2, xmm4           /* R += Y */                         \
    __asm psraw      xmm0, 6                                                   \
    __asm psraw      xmm1, 6                                                   \
    __asm psraw      xmm2, 6                                                   \
    __asm packuswb   xmm0, xmm0           /* B */                              \
    __asm packuswb   xmm1, xmm1           /* G */                              \
    __asm packuswb   xmm2, xmm2           /* R */                              \
  }

// Store 8 ARGB values.
#define STOREARGB __asm {                                                      \
    __asm punpcklbw  xmm0, xmm1           /* BG */                             \
    __asm punpcklbw  xmm2, xmm5           /* RA */                             \
    __asm movdqa     xmm1, xmm0                                                \
    __asm punpcklwd  xmm0, xmm2           /* BGRA first 4 pixels */            \
    __asm punpckhwd  xmm1, xmm2           /* BGRA next 4 pixels */             \
    __asm movdqu     0[edx], xmm0                                              \
    __asm movdqu     16[edx], xmm1                                             \
    __asm lea        edx,  [edx + 32]                                          \
  }

// 8 pixels.
// 4 UV values upsampled to 8 UV, mixed with 8 Y producing 8 ARGB (32 bytes).
__declspec(naked)
void I422ToARGBRow_SSSE3(const uint8_t* y_buf,
    const uint8_t* u_buf,
    const uint8_t* v_buf,
    uint8_t* dst_argb,
    const struct YuvConstants* yuvconstants,
    int width) 
{
    __asm 
    {
        push       esi
        push       edi
        push       ebx
        mov        eax, [esp + 12 + 4]   // Y
        mov        esi, [esp + 12 + 8]   // U
        mov        edi, [esp + 12 + 12]  // V
        mov        edx, [esp + 12 + 16]  // argb
        mov        ebx, [esp + 12 + 20]  // yuvconstants
        mov        ecx, [esp + 12 + 24]  // width
        sub        edi, esi
        pcmpeqb    xmm5, xmm5            // generate 0xffffffff for alpha

        convertloop :
        READYUV422
        YUVTORGB(ebx)
        STOREARGB

        sub        ecx, 8
        jg         convertloop

        pop        ebx
        pop        edi
        pop        esi
        ret
    }
}

ANY31C(I422ToARGBRow_Any_SSSE3, I422ToARGBRow_SSSE3, 1, 0, 4, 7)

extern "C" __declspec(dllexport)
int cpu_has_ssse3_libyuv_x86()
{
    return TestCpuFlag(kCpuHasSSSE3);
}

extern "C" __declspec(dllexport)
int bgra_to_yuv420_libyuv_x86(uint8_t *src_argb, int32_t src_stride_argb,
    uint8_t *dst_y, int32_t dst_stride_y,
    uint8_t *dst_u, int32_t dst_stride_u,
    uint8_t *dst_v, int32_t dst_stride_v,
    int32_t width, int32_t height)
{
    int32_t y;
    void(*ARGBToUVRow)(const uint8_t* src_argb0, int32_t src_stride_argb, uint8_t* dst_u, uint8_t* dst_v, int32_t width) = ARGBToUVRow_C;
    void(*ARGBToYRow)(const uint8_t* src_argb, uint8_t* dst_y, int32_t width) = ARGBToYRow_C;

    if (!src_argb || !dst_y || !dst_u || !dst_v || width <= 0 || height == 0)
    {
        return -1;
    }

    // Negative height means invert the image.
    if (height < 0)
    {
        height = -height;
        src_argb = src_argb + (height - 1) * src_stride_argb;
        src_stride_argb = -src_stride_argb;
    }

    if (TestCpuFlag(kCpuHasSSSE3))
    {
        ARGBToUVRow = ARGBToUVRow_Any_SSSE3;
        ARGBToYRow = ARGBToYRow_Any_SSSE3;
        if (IS_ALIGNED(width, 16))
        {
            ARGBToUVRow = ARGBToUVRow_SSSE3;
            ARGBToYRow = ARGBToYRow_SSSE3;
        }
    }

    for (y = 0; y < height - 1; y += 2)
    {
        ARGBToUVRow(src_argb, src_stride_argb, dst_u, dst_v, width);
        ARGBToYRow(src_argb, dst_y, width);
        ARGBToYRow(src_argb + src_stride_argb, dst_y + dst_stride_y, width);
        src_argb += src_stride_argb * 2;
        dst_y += dst_stride_y * 2;
        dst_u += dst_stride_u;
        dst_v += dst_stride_v;
    }
    if (height & 1)
    {
        ARGBToUVRow(src_argb, 0, dst_u, dst_v, width);
        ARGBToYRow(src_argb, dst_y, width);
    }
    return 0;
}

extern "C" __declspec(dllexport)
int yuv420_to_bgra_libyuv_x86(const uint8_t* src_y, int32_t src_stride_y,
    const uint8_t* src_u, int32_t src_stride_u,
    const uint8_t* src_v, int32_t src_stride_v,
    uint8_t* dst_argb, int32_t dst_stride_argb,
    int32_t width, int32_t height)
{
    // Convert I422 to ARGB with matrix
    int32_t y;
    void(*I422ToARGBRow)(const uint8_t* y_buf, const uint8_t* u_buf, const uint8_t* v_buf, uint8_t* rgb_buf, const struct YuvConstants* yuvconstants, int32_t width) = I422ToARGBRow_C;

    if (!src_y || !src_u || !src_v || !dst_argb || width <= 0 || height == 0)
    {
        return -1;
    }

    // Negative height means invert the image.
    if (height < 0)
    {
        height = -height;
        dst_argb = dst_argb + (height - 1) * dst_stride_argb;
        dst_stride_argb = -dst_stride_argb;
    }

    if (TestCpuFlag(kCpuHasSSSE3))
    {
        I422ToARGBRow = I422ToARGBRow_Any_SSSE3;
        if (IS_ALIGNED(width, 8))
        {
            I422ToARGBRow = I422ToARGBRow_SSSE3;
        }
    }

    for (y = 0; y < height; ++y)
    {
        I422ToARGBRow(src_y, src_u, src_v, dst_argb, &kYuvI601Constants, width);
        dst_argb += dst_stride_argb;
        src_y += src_stride_y;
        if (y & 1)
        {
            src_u += src_stride_u;
            src_v += src_stride_v;
        }
    }
    return 0;
}
