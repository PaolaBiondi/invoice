using eInvoice.domain.Adapters;
using eInvoice.domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace eInvoice.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly ILogger<InvoiceController> logger;
        private readonly IMediator mediator;

        public InvoiceController(ILogger<InvoiceController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpGet("PingInfra")]
        public async Task<IActionResult> PingInfra(CancellationToken cancellationToken)
        {
            var result = await mediator.Send<PingDto, bool>(null!, cancellationToken);

            if (result)
                return Ok("Ping successful.");

            return StatusCode(500, "Ping failed.");
        }

        [HttpPost("SendInvoice")]
        public async Task<IActionResult> SendInvoice(string invoice, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(invoice))
                return BadRequest("Invoice cannot be null or empty.");

            var invoiceDto = new ReadyInvoiceDto
            {
                InvoiceNumber = invoice
            };

            var result = await mediator.Send<ReadyInvoiceDto, bool>(invoiceDto, cancellationToken);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Failed to send invoice.");
        }

        // POST api/<InvoiceController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<InvoiceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<InvoiceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
