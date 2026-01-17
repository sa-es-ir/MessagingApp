using MessagingApp.Models;

namespace MessagingApp.Services
{
    public interface IConversationService
    {
        event Action? ConversationsChanged;

        Task<Message?> AddMessageAsync(string conversationId, string text, bool isFromUser);
        Task<Conversation> CreateOrGetConversationAsync(string userName);
        void DeleteConversation(string conversationId);
        List<Conversation> GetAllConversations();
        Conversation? GetConversation(string conversationId);
    }
}