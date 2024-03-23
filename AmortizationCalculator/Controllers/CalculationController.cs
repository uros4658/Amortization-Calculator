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
        public async Task<ActionResult<Loan>> AddAllPayments(Loan loan)
        {
            try
            {
                int loanID = await _calculationServices.AddLoan(loan);
                var payment = _calculationServices.MakePayment(loan, loanID);
                while (payment.AmountLeft > 0)
                {
                    payment = await _calculationServices.RegisterOneMonth(loan, payment);
                }
                return Ok(loan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("last-loan-id")]
        public async Task<IActionResult> GetLastLoanID()
        {
            try
            {
                var lastLoanID = await _calculationServices.getLastLoanID();
                return Ok(lastLoanID);
            }
            catch (Exception ex)
            {
                // Log the exception message
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{username}/loans")]
        public async Task<IActionResult> GetUserLoans(string username)
        {
            try
            {
                var loans = await _calculationServices.GetAllLoans(username);
                if (loans == null || !loans.Any())
                {
                    return NotFound();
                }

                return Ok(loans);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
