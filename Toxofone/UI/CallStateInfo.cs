namespace Toxofone.UI
{
    using Toxofone.Managers;

    public sealed class CallStateInfo
    {
        public CallStateInfo(int friendNumber, CallState callState)
        {
            this.FriendNumber = friendNumber;
            this.CallState = callState;
        }

        public int FriendNumber { get; private set; }
        public CallState CallState { get; private set; }
    }
}
