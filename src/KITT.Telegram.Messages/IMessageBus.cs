namespace KITT.Telegram.Messages
{
    public interface IMessageBus
    {
        Task SendAsync<TMessage>(TMessage message) where TMessage : class;
    }
}