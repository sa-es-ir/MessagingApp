using MessagingApp.Models;

namespace MessagingApp.Services;

public class ConversationService
{
    private readonly List<Conversation> _conversations = new();
    private readonly object _lock = new();

    // Event raised whenever conversations list or a conversation's metadata changes
    public event Action? ConversationsChanged;
    private void RaiseChanged() => ConversationsChanged?.Invoke();

    public List<Conversation> GetAllConversations()
    {
        lock (_lock)
        {
            return _conversations.OrderByDescending(c => c.LastMessageAt).ToList();
        }
    }

    public Conversation? GetConversation(string conversationId)
    {
        lock (_lock)
        {
            return _conversations.FirstOrDefault(c => c.Id == conversationId);
        }
    }

    public Conversation CreateOrGetConversation(string userName)
    {
        bool created = false;
        Conversation? conversation;
        lock (_lock)
        {
            var existingConversation = _conversations.FirstOrDefault(c => c.UserName == userName);
            if (existingConversation != null)
            {
                conversation = existingConversation;
            }
            else
            {
                conversation = new Conversation { UserName = userName, Title = "New Conversation" };
                _conversations.Add(conversation);
                created = true;
            }
        }
        if (created) RaiseChanged();
        return conversation!;
    }

    public void AddMessage(string conversationId, string text, bool isFromUser)
    {
        bool changed = false;
        lock (_lock)
        {
            var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
            if (conversation == null)
                return;

            var message = new Message
            {
                Text = text,
                IsFromUser = isFromUser,
                ConversationId = conversationId,
            };

            conversation.Messages.Add(message);
            conversation.LastMessageAt = DateTime.Now;

            if (isFromUser && conversation.Messages.Count(m => m.IsFromUser) == 1)
            {
                conversation.Title = text.Length > 50 ? text.Substring(0, 50) + "..." : text;
            }
            changed = true;
        }
        if (changed) RaiseChanged();
    }

    public void DeleteConversation(string conversationId)
    {
        bool deleted = false;
        lock (_lock)
        {
            var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
            if (conversation != null)
            {
                _conversations.Remove(conversation);
                deleted = true;
            }
        }
        if (deleted) RaiseChanged();
    }
}
