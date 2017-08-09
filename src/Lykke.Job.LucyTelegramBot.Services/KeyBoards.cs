using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public static class KeyBoards
    {
        public static readonly ReplyKeyboardMarkup MainKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton(BotCommands.GetPaid)             
            },
            new[]
            {
                new KeyboardButton(BotCommands.Contractors)
            },
            new[]
            {
                new KeyboardButton(BotCommands.NewbieGuide)
            }
        }, true);
    }
}