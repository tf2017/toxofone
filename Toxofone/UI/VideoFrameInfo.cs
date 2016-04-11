namespace Toxofone.UI
{
    using System;
    using System.Drawing;

    public sealed class VideoFrameInfo
    {
        public VideoFrameInfo(int friendNumber, Bitmap frame)
        {
            this.FriendNumber = friendNumber;
            this.Frame = frame;
        }

        public int FriendNumber { get; private set; }
        public Bitmap Frame { get; private set; }
    }
}