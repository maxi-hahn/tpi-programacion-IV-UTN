using Domain.Entity;
namespace Domain.Interface
{
    public interface IMercadoPagoService
    {
        Task<string> CreatePreference(Plan plan, Guid userId);

        Task ProcessPayment(string paymentId);

    }
}
