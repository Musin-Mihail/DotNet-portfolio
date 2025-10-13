namespace DotNet_portfolio.Services
{
    /// <summary>
    /// Interface for a message producer service.
    /// </summary>
    public interface IMessageProducer
    {
        /// <summary>
        /// Sends a message to the message queue.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message object to send.</param>
        void SendMessage<T>(T message);
    }
}
