using Application.Dtos.Responses;
using Application.Dtos.Request;

namespace Application.Interfaces
{
    public interface IInscriptionService
    {
        Task<InscriptionResult> Inscribe(InscriptionRequest request);
        Task<InscriptionResult> Unsubscribe(Guid userId, Guid classId);
        Task<IEnumerable<MyInscriptionResponse>> GetMyInscriptions(Guid userId); 
        Task<InscriptionResponse?> GetById(Guid id);

    }
}