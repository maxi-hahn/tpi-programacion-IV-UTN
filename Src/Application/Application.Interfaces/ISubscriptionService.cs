namespace Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task CheckExpiredSubscriptions();
    }
}
