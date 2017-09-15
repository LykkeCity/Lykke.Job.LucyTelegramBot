using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Domain;

namespace Lykke.Job.LucyTelegramBot.Core.Telegram
{
    public interface ITgEmployeeRepository
    {
        Task<ITgEmployee> Get(string id);
        Task<ITgEmployee> Find(string username);
        Task<ITgEmployee> Get(long chatId);
        Task AddUserAsync(string id, string email);
        Task TryRemoveAsync(string email);
        Task UpdateEmployeeInfo(string id, long chatId, string username, string firstName, string lastName);
        Task UpdateBio(long chatId, string bio);
        Task<IEnumerable<ITgEmployee>> GetEmployeesWithBio();
    }
}