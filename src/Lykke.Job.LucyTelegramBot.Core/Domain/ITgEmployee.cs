namespace Lykke.Job.LucyTelegramBot.Core.Domain
{
    public interface ITgEmployee
    {        
        long? ChatId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Bio { get; set; }
    }
}