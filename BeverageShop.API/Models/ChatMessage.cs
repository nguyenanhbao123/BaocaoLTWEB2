namespace BeverageShop.API.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsFromAdmin { get; set; } = false;
        public string? AttachmentUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public string RoomId { get; set; } = string.Empty; // For private chat rooms
    }
}
