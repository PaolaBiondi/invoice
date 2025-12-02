using eInvoice.infra.mojEracun.Dtos;
using eInvoice.infra.mojEracun.Scheme;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace eInvoice.Tests
{
    public class StubHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null || request.RequestUri is null)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }

            var response = request.RequestUri.AbsoluteUri switch
            {
                var uri when uri.EndsWith("send") && request.Method.Equals(HttpMethod.Post) && request.Content is not null =>
                    GenerateContent(ReturnSendResponse(request.Content.ReadAsStringAsync().Result.AsSpan())),
                var uri when uri.Contains("ping") && request.Method == HttpMethod.Get =>
                    GenerateContent(new PingResponse
                    {
                        Status = "OK",
                        Message = $"Service is up @{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}"
                    }),
                var uri when uri.Contains("/api/mps/check")
                             && request.Method == HttpMethod.Post
                             && request.Content is not null =>
                    CheckCompanyIdRequest(request.Content.ReadFromJsonAsync<CheckCompanyIdRequest>().Result),
                var uri when (uri.Contains("/api/fiscalization/markPaid") || uri.Contains("/api/fiscalization/markPaidWithoutElectronicId"))
                             && request.Method == HttpMethod.Post
                             && request.Content is not null => GenerateMarkPaidResponse(request.Content.ReadFromJsonAsync<MarkPaidRequest>().Result),
                _ => GenerateContent(new SendResponse
                {
                    Message = "Unknown endpoint.",
                    StatusCode = 404
                })
            };

            return Task.FromResult(new HttpResponseMessage((HttpStatusCode)response.code)
            {
                Content = response.content
            });
        }

        private InvoiceUploadResponse ReturnSendResponse(ReadOnlySpan<char> span)
        {
            if (span.Contains("\"file\":\"null\"".AsSpan(), StringComparison.CurrentCultureIgnoreCase))
            {
                return new InvoiceUploadResponse
                {
                    Status = 200,
                    Errors = new()
                    {
                        { "Document", new List<string> { "Dogodila se generalna greška.." } }
                    }
                };
            }

            return new InvoiceUploadResponse
            {
                Status = 200,
            };
        }

        private (StringContent content, HttpStatusCode code) GenerateContent<T>(T response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var json = JsonSerializer.Serialize(response);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return (content, statusCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>OK if it is AGCT, else BadRequest</returns>
        private (StringContent content, HttpStatusCode code) CheckCompanyIdRequest(CheckCompanyIdRequest? payload)
        {
            var result = payload?.IdentifierValue.Equals("80300395055") ?? false ? (new SendResponse { StatusCode = 200 }, HttpStatusCode.OK) : (new SendResponse
            {
                StatusCode = 400,
                TraceId = Guid.NewGuid().ToString(),
                Message = "Some error message"
            }, HttpStatusCode.BadRequest);

            var respone = GenerateContent(result.Item1, result.Item2);

            return respone;
        }

        private (StringContent content, HttpStatusCode code) GenerateMarkPaidResponse(MarkPaidRequest? payload)
        {
            var markPaidResponse = payload is null
                ? EinvoiceTestFactory.GenerateMarkPaidFailedResponse()
                : new MarkPaidResponse
                {
                    FiscalizationTimestamp = DateTimeOffset.Now, 
                    EncodedXml = "PD94 bWwgdmVyc 2 lvbj 0iMS4 wIiB ...",
                };

            var respone = GenerateContent(markPaidResponse, payload is null ? HttpStatusCode.BadRequest : HttpStatusCode.OK);
            return respone;
        }
    }
}
