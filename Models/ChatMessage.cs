public class ChatMessage
{
    public int Id { get; set; }

    public int SenderId { get; set; }
    public string SenderName { get; set; }

    public int ReceiverId { get; set; }
    public string ReceiverName { get; set; }

    public string MessageText { get; set; }

    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
}
