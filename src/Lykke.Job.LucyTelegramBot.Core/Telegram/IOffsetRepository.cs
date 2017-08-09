using System.Threading.Tasks;

namespace Lykke.Job.LucyTelegramBot.Core.Telegram
{
    public interface IOffsetRepository
    {
        Task<int> GetOffset();
        Task SetOffset(int offset);
    }
}