using System.Threading.Tasks;

namespace Lykke.Job.LucyTelegramBot.Core.Telegram
{
    public interface ITgUserState
    {
        long ChatId { get; set; }
        string ParentCommand { get; set; }
        string Command { get; set; }
    }

    public interface ITgUserStateRepository
    {
        Task<ITgUserState> GetStateAsync(long chatId);
        Task SetStateAsync(long chatId, string parentCommand, string command);
    }
}
