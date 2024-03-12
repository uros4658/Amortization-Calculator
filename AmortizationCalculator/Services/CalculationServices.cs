using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace AmortizationCalc.Services
{
    public class CalculationServices : ICalculationServices
    {
        private readonly IConfiguration _config;
        private Payment _payment;
        private MySqlConnection _connection;
        public CalculationServices(IConfiguration config)
        {
            _config = config;
            _payment = new Payment();
            _connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        // Adds the money needed to pay per month to pay out the loan to the object
        public double CalculateMonthlyCost(Loan loan)
        {
            if (loan.PaymentsPerYear == 0)
            {
                throw new ArgumentException("Payments per year cannot be zero.");
            }

            int principal = loan.LoanAmount - loan.DownPayment;
            int lengthOfLoanYears = loan.EndDate.Year - loan.StartDate.Year;

            double monthlyPayment = (principal * (loan.InterestRate / loan.PaymentsPerYear)) /
                 (1 - Math.Pow(1 + loan.InterestRate / loan.PaymentsPerYear, -1 * loan.PaymentsPerYear * lengthOfLoanYears));

            return monthlyPayment;
        }

        // Returns total amount of money payed to the bank 
        public double TotalAmountPayed(Loan loan)
        {
            int lengthOfLoanMonths = (loan.EndDate.Year - loan.StartDate.Year) * 12;
            return _payment.MonthlyPayment * lengthOfLoanMonths;
        }

        // Returns the amount of interest payed to the bank
        public double TotalInterestPayed(Loan loan)
        {
            return TotalAmountPayed(loan) - loan.LoanAmount;
        }

        // Returns how much did you pay towards paying of the loan that month
        public double PrincipalPayment(Payment payment, Loan loan)
        {
            double interest = (payment.AmountLeft * loan.InterestRate) / 12;
            double principalPayment = payment.MonthlyPayment - interest;

            return principalPayment;
        }


        // Returns how much money you payed to the bank that month
        public double InterestPayment(Loan loan)
        {
            return _payment.MonthlyPayment - PrincipalPayment(_payment, loan);
        }

        public Payment MakePayment(Loan loan)
        {
            _payment.AmountLeft = loan.LoanAmount - loan.DownPayment;
            _payment.LoanMonth = loan.StartDate;
            _payment.MonthlyPayment = CalculateMonthlyCost(loan);
            return _payment;
        }

        public async Task<Payment> RegisterOneMonth(Loan loan, Payment payment)
        {
            payment.AmountLeft -= PrincipalPayment(payment, loan);
            payment.Interest = InterestPayment(loan);
            payment.Principal = PrincipalPayment(payment, loan);
            payment.LoanMonth = payment.LoanMonth.AddMonths(1);
            _payment = payment;

            await _connection.ExecuteAsync("INSERT INTO payment (amountleft, principal, interest, loanmonth) " +
                "VALUES (@amountleft, @principal, @interest, @loanmonth)", payment);
            return payment;
        }

        public Task<Payment> RegisterOneMonth(Loan loan)
        {
            throw new NotImplementedException();
        }
    }
}
