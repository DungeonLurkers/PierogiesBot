using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PierogiesBot.Discord.Services
{
    /// <summary>
    /// Discord channel subscribing service
    /// </summary>
    public interface IChannelSubscribeService
    {
        /// <summary>
        /// Loads all saved subscriptions
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task LoadSubscriptionsAsync();

        /// <summary>
        /// Subscribes to given channel
        /// </summary>
        /// <param name="channel">Channel to subscribe.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task SubscribeAsync(SocketGuildChannel channel);

        /// <summary>
        /// Unsubscribes from given channel
        /// </summary>
        /// <param name="channel">Channel to unsubscribe.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UnsubscribeAsync(SocketGuildChannel channel);
    }
}