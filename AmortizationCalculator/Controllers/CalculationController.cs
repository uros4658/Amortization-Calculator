using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AmortizationCalc.Models;
using AmortizationCalc.Services;
using System.Threading.Tasks;
using AmortizationCalc.Interfaces;

namespace AmortizationCalc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    public class CalculationController : ControllerBase
    {
        private readonly ICalculationServices _calculationServices;

        public CalculationController(ICalculationServices calculationServices)
        {
            _calculationServices = calculationServices;
        }


        [HttpPost("add-amortization-plan")]
        public async Task<ActionResult<Payment>> AddAllPayments(Loan loan)
        {
            try
            {
                int loanID = await _calculationServices.AddLoan(loan);
                var payment = _calculationServices.MakePayment(loan, loanID);
                while (payment.LoanMonth < loan.EndDate)
                {
                    payment = await _calculationServices.RegisterOneMonth(loan, payment);
                }
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
