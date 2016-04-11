namespace Toxofone.Utils
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using SharpTox.Av;

    public static class VideoUtils
    {
        public static bool CpuHasSsse3
        {
            get { return NativeMethods.CpuHasSsse3_LibYuvX86() != 0; }
        }

        public static ToxAvVideoFrame BitmapToToxAvFrame(Bitmap bmp, bool withLibYuv = false)
        {
            if (bmp == null)
            {
                return null;
            }
            if (bmp.Height <= 0 || bmp.Height % 2 > 0)
            {
                return null;
            }
            if (bmp.Width <= 0 || bmp.Width % 2 > 0)
            {
                return null;
            }

            int halfHeight = bmp.Height / 2;
            int halfWidth = bmp.Width / 2;

            byte[] y = new byte[bmp.Height * bmp.Width];
            byte[] u = new byte[halfHeight * halfWidth];
            byte[] v = new byte[halfHeight * halfWidth];

            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                if (withLibYuv)
                {
                    int argb_stride = bmp.Width * 4;
                    int y_stride = bmp.Width;
                    int u_stride = halfWidth;
                    int v_stride = halfWidth;
                    NativeMethods.BgraToYuv420_LibYuvX86(bitmapData.Scan0, argb_stride, y, y_stride, u, u_stride, v, v_stride, bmp.Width, bmp.Height);
                }
                else
                {
                    NativeMethods.BgraToYuv420(bmp.Width, bmp.Height, bitmapData.Scan0, y, u, v);
                }
            }
            finally
            {
                bmp.UnlockBits(bitmapData);
            }

            return new ToxAvVideoFrame(bmp.Width, bmp.Height, y, u, v);
        }

        public static Bitmap ToxAvFrameToBitmap(ToxAvVideoFrame toxFrame, bool withLibYuv = false)
        {
            if (toxFrame == null)
            {
                return null;
            }
            if (toxFrame.Height <= 0 || toxFrame.Height % 2 > 0)
            {
                return null;
            }
            if (toxFrame.Width <= 0 || toxFrame.Width % 2 > 0)
            {
                return null;
            }

            Bitmap bmp = new Bitmap(toxFrame.Width, toxFrame.Height, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            try
            {
                if (withLibYuv)
                {
                    int argb_stride = toxFrame.Width * 4;
                    NativeMethods.Yuv420ToBgra_LibYuvX86(toxFrame.Y, toxFrame.YStride, toxFrame.U, toxFrame.UStride, toxFrame.V, toxFrame.VStride, bitmapData.Scan0, argb_stride, toxFrame.Width, toxFrame.Height);
                }
                else
                {
                    NativeMethods.Yuv420ToBgra(toxFrame.Width, toxFrame.Height, toxFrame.Y, toxFrame.U, toxFrame.V, toxFrame.YStride, toxFrame.UStride, toxFrame.VStride, bitmapData.Scan0);
                }
            }
            finally
            {
                bmp.UnlockBits(bitmapData);
            }

            return bmp;
        }

        private static class NativeMethods
        {
            [DllImport("tfutils.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "bgra_to_yuv420")]
            public static extern void BgraToYuv420(int width, int height, IntPtr in_bgra, byte[] out_y, byte[] out_u, byte[] out_v);

            [DllImport("tfutils.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "yuv420_to_bgra")]
            public static extern void Yuv420ToBgra(int width, int height, byte[] in_y, byte[] in_u, byte[] in_v, int y_stride, int u_stride, int v_stride, IntPtr out_bgra);

            [DllImport("tfutils.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cpu_has_ssse3_libyuv_x86")]
            public static extern int CpuHasSsse3_LibYuvX86();

            [DllImport("tfutils.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "bgra_to_yuv420_libyuv_x86")]
            public static extern int BgraToYuv420_LibYuvX86(IntPtr src_argb, int src_stride_argb, 
                byte[] dst_y, int dst_stride_y, 
                byte[] dst_u, int dst_stride_u,
                byte[] dst_v, int dst_stride_v,
                int width, int height);

            [DllImport("tfutils.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "yuv420_to_bgra_libyuv_x86")]
            public static extern int Yuv420ToBgra_LibYuvX86(byte[] src_y, int src_stride_y,
                byte[] src_u, int src_stride_u,
                byte[] src_v, int src_stride_v, 
                IntPtr dst_argb, int dst_stride_argb, 
                int width, int height);
        }
    }
}
