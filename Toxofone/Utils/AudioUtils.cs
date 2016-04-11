namespace Toxofone.Utils
{
    using System;

    public static class AudioUtils
    {
        public static short[] BytesToShorts(byte[] bytes)
        {
            short[] shorts = new short[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, shorts, 0, bytes.Length);
            return shorts;
        }

        public static byte[] ShortsToBytes(short[] shorts)
        {
            byte[] bytes = new byte[shorts.Length * 2];
            Buffer.BlockCopy(shorts, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
