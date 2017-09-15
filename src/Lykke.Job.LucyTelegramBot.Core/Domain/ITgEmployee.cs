namespace Lykke.Job.LucyTelegramBot.Core.Domain
{
    public interface ITgEmployee
    {        
        long? ChatId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string Bio { get; set; }
    }

    public static class TgEmployeeExt
    {
        public static string FullName(this ITgEmployee src)
        {
            return $"{src.FirstName} {src.LastName}".TrimEnd();
        }
    }
}