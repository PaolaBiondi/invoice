using eInvoice.domain.Adapters;

namespace eInvoice.api
{
    public class EInvoiceMediator: IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        public EInvoiceMediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        {
            var handler = _serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for request type {typeof(TRequest).FullName}");
            }

            return await handler.Handle(request);
        }
    }
}
