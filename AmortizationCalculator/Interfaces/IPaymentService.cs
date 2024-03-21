using AmortizationCalc.Models;
using AmortizationCalculator.Models;

namespace AmortizationCalculator.Interfaces
{
    public interface IPaymentService
    {
        public Task DeleteOtherPayments(int id);
        Task<Payment> MakeDifferentPayment(Payment payment);
        Task<Payment> RegisterAdjustedPayment(Payment payment);
        Task<Payment> MissedPaymentRegister(Payment payment);
        Task<Payment> RegisterAdjustedPaymentInterestDouble(Payment payment);
        Task<double> CalculateMonthlyCost(Payment payment);
        Task<IEnumerable<Payment>> GetAllPayemnts();
        Task<Payment> CreatePaymentFromNewPayment(NewPayment newPayment);
    }
}
