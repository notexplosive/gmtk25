namespace OutLoop.Core
{
    public class DirectMessage
    {
        public Account Sender { get; }
        public string Message { get; }

        public DirectMessage(Account sender, string message)
        {
            Message = message;
            Sender = sender;
        }
    }
}