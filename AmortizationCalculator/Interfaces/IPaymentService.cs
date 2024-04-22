using AmortizationCalc.Models;
using AmortizationCalculator.Models;

namespace AmortizationCalculator.Interfaces
{
    public interface IPaymentService
    {
        public Task DeleteOtherPayments(int id);
        Task<Payment> MakeDifferentPayment(Payment payment);
        Task<Payment> RegisterAdjustedPayment(Payment payment, MiscCost[] miscCost);
        Task<Payment> MissedPaymentRegister(Payment payment);
        Task<Payment> RegisterAdjustedPaymentInterestDouble(Payment payment);
        Task<double> CalculateMonthlyCost(Payment payment);
        Task<IEnumerable<Payment>> GetAllPayments(int loanID);
        Task<Payment> CreatePaymentFromNewPayment(NewPayment newPayment);
        Task<Payment> AddMiscCost(Payment payment, MiscCost misccost);
        Task<MiscCost[]> GetMisc(int LoanID);
        Task<double> CalculateEffectiveInterestRate(int loanID);
    }
}
