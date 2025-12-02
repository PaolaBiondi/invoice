namespace eInvoice.domain.Repositories
{
    public interface ICheckCompanyRepository
    {
        Task<IReadOnlyList<string>?> CheckCompanyIdsInAmsAsync(CancellationToken cancellationToken = default);
        Task UpdateCompanyIdAsync(string companyId, bool isRegistered);
    }
}
