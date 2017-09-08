using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.LucyTelegramBot.AzureRepositories.Telegram
{
    public class TgUserStateEntity : TableEntity, ITgUserState
    {
        public static string GeneratePartition()
        {
            return "UserState";
        }

        public static string GenerateRowKey(long id)
        {
            return id.ToString();
        }

        public static TgUserStateEntity Create(long id, string parentCommand, string command)
        {
            return new TgUserStateEntity
            {
                PartitionKey = GeneratePartition(),
                RowKey = GenerateRowKey(id),
                ChatId = id,
                ParentCommand = parentCommand,
                Command = command
            };
        }

        public long ChatId { get; set; }
        public string ParentCommand { get; set; }
        public string Command { get; set; }
    }

    public class TgUserStateRepository : ITgUserStateRepository
    {
        private readonly INoSQLTableStorage<TgUserStateEntity> _tableStorage;

        public TgUserStateRepository(INoSQLTableStorage<TgUserStateEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<ITgUserState> GetStateAsync(long chatId)
        {
            return await _tableStorage.GetDataAsync(TgUserStateEntity.GeneratePartition(), TgUserStateEntity.GenerateRowKey(chatId));
        }

        public async Task SetStateAsync(long chatId, string parentCommand, string command)
        {
            await _tableStorage.InsertOrReplaceAsync(TgUserStateEntity.Create(chatId, parentCommand, command));
        }
    }
}
