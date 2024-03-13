using AmortizationCalc.Models;

namespace AmortizationCalc.Interfaces
{
    public interface ICalculationServices
    {
        double CalculateMonthlyCost(Loan loan);
        double TotalAmountPayed(Loan loan);
        double TotalInterestPayed(Loan loan);
        double PrincipalPayment(Payment payment, Loan loan);
        double InterestPayment(Loan loan);
        Payment MakePayment(Loan loan, int ID);
        Task<Payment> RegisterOneMonth(Loan loan, Payment payment);
        Task<int> AddLoan(Loan loan);
    }
}
