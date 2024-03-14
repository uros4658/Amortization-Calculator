using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using AmortizationCalc.Services;
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
        [HttpPost("Change This Months Payment")]
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

        [HttpPost("Missed payment Extend Loan")]
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

        [HttpPost("Missed payment same length")]
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

        [HttpPost("Change, Delete and make full payment plan with new monthly payment")]
        public async Task<IActionResult> ChangeDeleteAndMake(Payment payment)
        {
            try
            {
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
    }

}
