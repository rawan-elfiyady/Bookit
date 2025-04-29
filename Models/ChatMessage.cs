public class ChatMessage
{
    public int Id { get; set; }

    public string SenderId { get; set; }
    public string SenderName { get; set; }

    public string ReceiverId { get; set; }
    public string ReceiverName { get; set; }

    public string MessageText { get; set; }

    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
}
