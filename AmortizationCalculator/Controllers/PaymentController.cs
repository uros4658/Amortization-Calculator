using AmortizationCalc.Models;
using AmortizationCalculator.Interfaces;
using AmortizationCalculator.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("get-all-payments")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllPayments()
        {
            try
            {
                var users = await _paymentService.GetAllPayemnts();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Change-This-Months-Payment")]
        public async Task<IActionResult> ChangeMonthlyPayment(Payment payment)
        {

            try
            {
                await _paymentService.MakeDifferentPayment(payment);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Missed-payment-Extend-Loan")]
        public async Task<IActionResult> MissedPaymentExtendLoan(Payment payment)
        {
            try
            {
                payment = await _paymentService.MissedPaymentRegister(payment);
                await DeleteOtherPayments(payment.Id);
                payment = await _paymentService.RegisterAdjustedPaymentInterestDouble(payment);
                while (payment.AmountLeft > 0)
                {
                    payment = await _paymentService.RegisterAdjustedPayment(payment);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Missed-payment-same-length")]
        public async Task<IActionResult> MissedPaymentSameLength(Payment payment)
        {
            try
            {
                payment = await _paymentService.MissedPaymentRegister(payment);
                await DeleteOtherPayments(payment.Id);
                payment.MonthlyPayment = await _paymentService.CalculateMonthlyCost(payment);
                while (payment.AmountLeft > 0)
                {
                    payment = await _paymentService.RegisterAdjustedPayment(payment);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Change-Delete-and-make-full-payment-new-monthly")]
        public async Task<IActionResult> ChangeDeleteAndMake(NewPayment newPayment)
        {
            try
            {
                Payment payment = await _paymentService.CreatePaymentFromNewPayment(newPayment);
                await ChangeMonthlyPayment(payment);
                await DeleteOtherPayments(payment.Id);
                while (payment.AmountLeft > 0)
                {
                    payment = await _paymentService.RegisterAdjustedPayment(payment);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("Different payment each n months")]
        public async Task<IActionResult> DifferenpaymentEachNMonths(PaymentEachN paymentEachN)
        {
            try
            {
                Payment paymentUsual = paymentEachN.paymentUsual;
                Payment paymentEach = paymentEachN.paymentNMonths;
                int n = paymentEachN.nMonths;
                int i = 0;
                while (paymentUsual.AmountLeft > 0)
                {
                    if(i%n == 0)
                    {
                        paymentEach = await _paymentService.RegisterAdjustedPayment(paymentEach);
                        paymentUsual.AmountLeft = paymentUsual.AmountLeft - paymentEach.Principal;
                    }
                    else
                    {
                        paymentUsual = await _paymentService.RegisterAdjustedPayment(paymentUsual);
                    }
                }
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
