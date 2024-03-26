using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AmortizationCalc.Models;
using AmortizationCalc.Services;
using System.Threading.Tasks;
using AmortizationCalc.Interfaces;
using AmortizationCalculator.Models;

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
        [HttpPost("Create-loan")]
        public async Task<IActionResult> CreateLoan(Loan loan)
        {
            try
            {
                int loanID = await _calculationServices.AddLoan(loan);
                loan.Id = loanID;
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("add-payment-plan")]
        public async Task<ActionResult<Loan>> AddAllPayments(Loan loan)
        {
            try
            {
                var lastLoanID = await _calculationServices.getLastLoanID();
                var payment = _calculationServices.MakePayment(loan, lastLoanID);
                MiscCost[] miscCost = await _calculationServices.GetMisc(lastLoanID);
                while (payment.AmountLeft > 0)
                {
                    payment = await _calculationServices.RegisterOneMonth(loan, payment, miscCost);
                }
                return Ok(loan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Create-misc")]
        public async Task<IActionResult> CreateMiscCost(MiscCost miscCost)
        {
            try
            {
                var lastLoanID = await _calculationServices.getLastLoanID();
                miscCost.LoanID = lastLoanID;
                await _calculationServices.InsertMisc(miscCost);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return StatusCode(500, "Internal server error");
            }
        }

        // Add other API endpoints here


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
