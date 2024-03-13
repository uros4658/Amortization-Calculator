using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using AmortizationCalculator.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmortizationCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOtherPayments(int id)
        {
            try
            {
                await _paymentService.DeleteOtherPayments(id);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
