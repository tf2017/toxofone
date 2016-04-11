namespace Toxofone.UI
{
    public sealed class FriendRequestInfo
    {
        public FriendRequestInfo(string publicKey, string message)
        {
            this.PublicKey = publicKey;
            this.Message = message;
        }

        public string PublicKey { get; private set; }
        public string Message { get; private set; }
    }
}
