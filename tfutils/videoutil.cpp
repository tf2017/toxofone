#include <stdint.h>

/*
 This code was taked from uTox, https://github.com/notsecure/uTox
 It's licensed under GPLv3
*/

__inline
static int imax(int a, int b)
{
	return (a > b) ? a : b;
}

__inline
static uint8_t rgb_to_y(int r, int g, int b)
{
	int y = ((9798 * r + 19235 * g + 3736 * b) >> 15);
	return y > 255 ? 255 : y < 0 ? 0 : y;
}

__inline
static uint8_t rgb_to_u(int r, int g, int b)
{
	int u = ((-5538 * r + -10846 * g + 16351 * b) >> 15) + 128;
	return u > 255 ? 255 : u < 0 ? 0 : u;
}

__inline
static uint8_t rgb_to_v(int r, int g, int b)
{
	int v = ((16351 * r + -13697 * g + -2664 * b) >> 15) + 128;
	return v > 255 ? 255 : v < 0 ? 0 : v;
}

extern "C" __declspec(dllexport)
void bgra_to_yuv420(int32_t width, int32_t height, uint8_t *in_bgra, uint8_t *out_y, uint8_t *out_u, uint8_t *out_v)
{
	for (int32_t y = 0; y < height; y += 2)
	{
		uint8_t *start_ptr = in_bgra + 4 * (y * width);
		uint8_t *in_ptr = start_ptr;

		// first scan line
		for (int32_t x = 0; x < width; x++)
		{
			uint8_t b = *in_ptr++;
			uint8_t g = *in_ptr++;
			uint8_t r = *in_ptr++;
			in_ptr++;  // alpha

			*out_y++ = rgb_to_y(r, g, b);
		}

		// second scan line
		for (int32_t x = 0; x < width; x += 2)
		{
			uint8_t b11 = *start_ptr++;
			uint8_t g11 = *start_ptr++;
			uint8_t r11 = *start_ptr++;
			start_ptr++;  // alpha

			uint8_t b12 = *start_ptr++;
			uint8_t g12 = *start_ptr++;
			uint8_t r12 = *start_ptr++;
			start_ptr++;  // alpha

			uint8_t b21 = *in_ptr++;
			uint8_t g21 = *in_ptr++;
			uint8_t r21 = *in_ptr++;
			in_ptr++;  // alpha

			*out_y++ = rgb_to_y(r21, g21, b21);

			uint8_t b22 = *in_ptr++;
			uint8_t g22 = *in_ptr++;
			uint8_t r22 = *in_ptr++;
			in_ptr++;  // alpha

			*out_y++ = rgb_to_y(r22, g22, b22);

			uint8_t b = ((int)b11 + (int)b12 + (int)b21 + (int)b22 + 2) / 4;
			uint8_t g = ((int)g11 + (int)g12 + (int)g21 + (int)g22 + 2) / 4;
			uint8_t r = ((int)r11 + (int)r12 + (int)r21 + (int)r22 + 2) / 4;

			*out_u++ = rgb_to_u(r, g, b);
			*out_v++ = rgb_to_v(r, g, b);
		}
	}
}

extern "C" __declspec(dllexport)
void yuv420_to_bgra(int32_t width, int32_t height, const uint8_t *in_y, const uint8_t *in_u, const uint8_t *in_v, int32_t y_stride, int32_t u_stride, int32_t v_stride, uint8_t *out_bgra)
{
	for (int32_t y = 0; y < height; y++)
	{
		int32_t y_offset = y * y_stride;
		int32_t u_offset = (y / 2) * u_stride;
		int32_t v_offset = (y / 2) * v_stride;
		uint8_t *out_ptr = out_bgra + 4 * (y * width);

		for (int32_t x = 0; x < width; x++)
		{
			int32_t t_y = imax(in_y[y_offset + x], 16);
			int32_t t_u = in_u[u_offset + (x / 2)];
			int32_t t_v = in_v[v_offset + (x / 2)];

			int32_t t_y_2 = 298 * (t_y - 16);
			int32_t r = (t_y_2 + 409 * (t_v - 128) + 128) >> 8;
			int32_t g = (t_y_2 - 100 * (t_u - 128) - 208 * (t_v - 128) + 128) >> 8;
			int32_t b = (t_y_2 + 516 * (t_u - 128) + 128) >> 8;

			*out_ptr++ = b > 255 ? 255 : b < 0 ? 0 : b;
			*out_ptr++ = g > 255 ? 255 : g < 0 ? 0 : g;
			*out_ptr++ = r > 255 ? 255 : r < 0 ? 0 : r;
			*out_ptr++ = 255;  // alpha
		}
	}
}
