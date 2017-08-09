using System;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Job.LucyTelegramBot.Core.Domain;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.LucyTelegramBot.AzureRepositories.Telegram
{
    public class TgEmployee : TableEntity, ITgEmployee
    {
        public static string GeneratePartition()
        {
            return "employee";
        }

        public static string GenerateRowKey(string id)
        {
            return id;
        }

        public static TgEmployee Create(string id, string email)
        {
            return new TgEmployee
            {
                PartitionKey = GeneratePartition(),
                RowKey = GenerateRowKey(id),
                Email = email
            };
        }
        
        public long? ChatId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
    }

    public class TgEmployeeRepository : ITgEmployeeRepository
    {
        private readonly INoSQLTableStorage<TgEmployee> _tableStorage;

        public TgEmployeeRepository(
            INoSQLTableStorage<TgEmployee> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<ITgEmployee> Get(string id)
        {
            return await _tableStorage.GetDataAsync(TgEmployee.GeneratePartition(), TgEmployee.GenerateRowKey(id));
        }

        public async Task<ITgEmployee> Find(string username)
        {
            return (await _tableStorage.GetDataAsync(e => e.UserName == username)).SingleOrDefault();
        }

        public async Task<ITgEmployee> Get(long chatId)
        {
            return (await _tableStorage.GetDataAsync(e => e.ChatId == chatId)).SingleOrDefault();
        }

        public async Task AddUserAsync(string id, string email)
        {
            var existing = await _tableStorage.GetDataAsync(TgEmployee.GeneratePartition(),
                TgEmployee.GenerateRowKey(id));

            if (existing != null)
                return;

            await _tableStorage.InsertAsync(TgEmployee.Create(id, email));            
        }

        public async Task TryRemoveAsync(string id)
        {
            var existing = await _tableStorage.GetDataAsync(TgEmployee.GeneratePartition(),
                TgEmployee.GenerateRowKey(id));

            if (existing != null)
                await _tableStorage.DeleteAsync(TgEmployee.GeneratePartition(),
                    TgEmployee.GenerateRowKey(id));
        }

        public async Task UpdateEmployeeInfo(string id, long chatId, string username, string firstName, string lastName)
        {
            var emp = await _tableStorage.GetDataAsync(TgEmployee.GeneratePartition(), TgEmployee.GenerateRowKey(id));

            emp.ChatId = chatId;
            emp.FirstName = firstName;
            emp.LastName = lastName;
            emp.UserName = username;

            await _tableStorage.InsertOrMergeAsync(emp);
        }

        public async Task UpdateBio(long chatId, string bio)
        {
            var emp = (await _tableStorage.GetDataAsync(e => e.ChatId == chatId)).SingleOrDefault();

            emp.Bio = bio;            

            await _tableStorage.InsertOrMergeAsync(emp);
        }
    }
}