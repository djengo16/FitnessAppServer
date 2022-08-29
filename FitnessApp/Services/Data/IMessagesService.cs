using FitnessApp.Dto.Message;

namespace FitnessApp.Services.Data
{
    public interface IMessagesService
    {
        Task CreateAsync(MessageResponseDTO message);
    }
}
