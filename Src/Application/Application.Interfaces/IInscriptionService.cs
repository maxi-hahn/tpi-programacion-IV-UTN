using Application.Dtos.Responses;
using Application.Dtos.Request;

namespace Application.Interfaces
{
    public interface IInscriptionService
    {
        Task<InscriptionResult> Inscribe(Guid userId, InscriptionRequest request);
        Task<InscriptionResult> Unsubscribe(Guid userId, Guid scheduleId);
        Task<IEnumerable<MyInscriptionResponse>> GetMyInscriptions(Guid userId);
    }
}