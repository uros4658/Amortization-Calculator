using AmortizationCalc.Models;
using AmortizationCalculator.Models;

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
        Task<Payment> RegisterOneMonth(Loan loan, Payment payment, MiscCost[] miscCost);
        Task<int> AddLoan(Loan loan);
        Task<Payment> AdjustPayment(Payment payment, Loan loan);
        Task<IEnumerable<Loan>> GetAllLoans(string username);
        Task<int> getLastLoanID();
        Task<MiscCost[]> GetMisc(int LoanID);
        Task InsertMisc(MiscCost miscCost);
    }
}
