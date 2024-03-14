using AmortizationCalc.Models;

namespace AmortizationCalculator.Interfaces
{
    public interface IPaymentService
    {
        public Task DeleteOtherPayments(int id);
        Task<Payment> MakeDifferentPayment(Payment payment);
        Task<Payment> RegisterAdjustedPayment(Payment payment);
        Task<Payment> MissedPaymentRegister(Payment payment);
        Task<Payment> RegisterAdjustedPaymentInterestDouble(Payment payment);

    }

}
