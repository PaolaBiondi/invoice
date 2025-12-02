namespace eInvoice.domain.Repositories
{
    public interface IPurgeRepository
    {
        Task<bool> PurgeAsync(ushort months, CancellationToken cancellationToken = default);
    }
}
