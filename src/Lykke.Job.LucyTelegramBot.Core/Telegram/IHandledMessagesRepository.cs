using System.Threading.Tasks;

namespace Lykke.Job.LucyTelegramBot.Core.Telegram
{
    public interface IHandledMessagesRepository
    {
        /// <summary>
        /// Tries to insert message id
        /// </summary>
        /// <param name="id">message id</param>
        /// <returns>true, if not handled yet</returns>
        Task<bool> TryHandleMessage(int id);
    }
}