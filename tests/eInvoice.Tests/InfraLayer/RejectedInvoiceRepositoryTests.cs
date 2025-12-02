
using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eInvoice.Tests.InfraLayer
{
    public class RejectedInvoiceRepositoryTests : IAsyncLifetime
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
        public void InitializePaidInvoiceRepositoryTests()
        {
            var repository = _host?.Services.GetRequiredService<IPaidInvoiceRepository>();

            Assert.NotNull(repository);
        }

        [Theory]
        [InlineData("996090")]
        [InlineData("4-P1-1", false)]
        public async Task GetRejectedInvoiceAsync_success(string invoiceId, bool expected = true)
        {
            var repo = _host?.Services.GetRequiredService<IRejectedInvoiceRepository>();

            var sut = await repo!.GetRejectedInvoiceAsync(invoiceId);

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
            var request = EinvoiceTestFactory.GenerateRejectedInvoiceDto("996090", true, true);

            var sut = _host?.Services.GetRequiredService<IRejectedInvoiceRepository>();

            var result = await sut!.UpdateRejectedInvoiceAsync(request);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(GenerateFakeRejectedInvoiceDtos))]
        public async Task UpdateRejectedInvoiceAsync_failed(RejectedInvoiceDto request, Type expected)
        {
            var sut = _host?.Services.GetRequiredService<IRejectedInvoiceRepository>();

            var exception = await Record.ExceptionAsync(async () => await sut!.UpdateRejectedInvoiceAsync(request));

            Assert.IsType(expected, exception);
        }

        public static IEnumerable<object[]> GenerateFakeRejectedInvoiceDtos()
        {
            return new List<object[]>
            {
                new object[] {EinvoiceTestFactory.GenerateRejectedInvoiceDto("izuxuhnng", true, true), typeof(InvalidOperationException) },
                new object[] {EinvoiceTestFactory.GenerateRejectedInvoiceDto(isRegistered: true,  hasMessage: true), typeof(ArgumentNullException) }
            };
        }
    }
}
