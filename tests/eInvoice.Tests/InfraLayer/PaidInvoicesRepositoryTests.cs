using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace eInvoice.Tests.InfraLayer
{
    public class PaidInvoicesRepositoryTests : IAsyncLifetime
    {
        private IHost? _host;

        public async Task DisposeAsync()
        {
            if (_host is null) return;

            await _host.StopAsync();
            _host?.Dispose();
        }

        public async Task InitializeAsync()
        {
            _host = await EinvoiceTestFactory.CreateTestHost();
        }

        [Fact]
        public void InitializePaidInvoicesRepositoryTests()
        {
            var repository = _host?.Services.GetRequiredService<IPaidInvoiceRepository>();

            Assert.NotNull(repository);
        }

        [Theory]
        [InlineData("996090")]
        [InlineData("4-P1-1", false)]
        public async Task GetPaidInvoiceAsync_success(string invoiceId, bool expected = true)
        {
            var repo = _host?.Services.GetRequiredService<IPaidInvoiceRepository>();

            var sut = await repo!.GetPaidInvoiceAsync(invoiceId);

            if (!expected)
            {
                Assert.Null(sut);
                return;
            }

            Assert.NotNull(sut);
            Assert.Equal(invoiceId, sut.InvoiceNumber);
            Assert.NotNull(sut.CompanyId);
        }

        [Fact]
        public async Task UpdatePaidInvoiceAsync_success()
        {
            var request = EinvoiceTestFactory.GeneratePaidInvoiceDto("996090", true, true);

            var sut = _host?.Services.GetRequiredService<IPaidInvoiceRepository>();

            var result = await sut!.UpdatePaidInvoiceAsync(request);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(GenerateFakePaidInvoiceDtos))]
        public async Task UpdatePaidInvoiceAsync_failed_InvoiceNotFound(PaidInvoiceDto request, Type expected)
        {
            var sut = _host?.Services.GetRequiredService<IPaidInvoiceRepository>();

            var exception = await Record.ExceptionAsync(async () => await sut!.UpdatePaidInvoiceAsync(request));
            
            Assert.IsType(expected, exception);
        }

        public static IEnumerable<object[]> GenerateFakePaidInvoiceDtos()
        {
            return new List<object[]>
            {
                new object[] {EinvoiceTestFactory.GeneratePaidInvoiceDto("izuxuhnng", true, true), typeof(InvalidOperationException) },
                new object[] {EinvoiceTestFactory.GeneratePaidInvoiceDto(isRegistered: true,  hasMessage: true), typeof(ArgumentNullException) }
            };
        }
    }
}
