using Application.Dtos.Responses;
using Application.Dtos.Request;

namespace Application.Interfaces
{
    public interface IInscriptionService
    {
        Task<InscriptionResponse?> Inscribe(InscriptionRequest request);
        Task<InscriptionResponse?> GetById(Guid id);

    }
}