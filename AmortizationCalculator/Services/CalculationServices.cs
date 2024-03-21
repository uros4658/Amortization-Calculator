using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using Dapper;
using MySql.Data.MySqlClient;

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

        // Returns the real interest paid
        public double RealInterestRatePaid(Loan loan, double inflationRate)
        {
            double nominalInterestPaid = TotalInterestPayed(loan);

            int lengthOfLoanYears = loan.EndDate.Year - loan.StartDate.Year;

            double totalInflation = Math.Pow(1 + inflationRate, lengthOfLoanYears) - 1;

            double realInterestPaid = nominalInterestPaid - (loan.LoanAmount * totalInflation);

            return realInterestPaid;
        }


        // Returns how much money you payed to the bank that month
        public double InterestPayment(Loan loan)
        {
            return _payment.MonthlyPayment - PrincipalPayment(_payment, loan);
        }

        // Makes _payment have all the neccessary paramaters for the start
        public Payment MakePayment(Loan loan, int ID)
        {
            _payment.AmountLeft = loan.LoanAmount - loan.DownPayment;
            _payment.LoanMonth = loan.StartDate;
            _payment.MonthlyPayment = CalculateMonthlyCost(loan);
            _payment.LoanID = ID;
            return _payment;
        }
        public async Task<Payment> AdjustPayment(Payment payment, Loan loan)
        {

            payment.Interest = InterestPayment(loan);
            payment.Principal = PrincipalPayment(payment, loan);
            payment.AmountLeft = payment.AmountLeft - payment.Principal;
            payment.LoanMonth = payment.LoanMonth.AddMonths(12 / loan.PaymentsPerYear);
            _payment = payment;

            if (payment.AmountLeft < 0)
            {
                payment.Principal += payment.AmountLeft;
                payment.AmountLeft = 0;
            }

            return payment;
        }
        public async Task<Payment> RegisterOneMonth(Loan loan, Payment payment)
        {
            string sql = "INSERT INTO payment (amountleft, monthlypayment, principal, interest, loanmonth, loanID) " +
                "VALUES (@amountleft, @monthlypayment, @principal, @interest, @loanmonth, @loanID)";

            payment = await AdjustPayment(payment, loan);

            if (payment.AmountLeft + payment.Interest < payment.MonthlyPayment)
            {
                payment.MonthlyPayment = payment.AmountLeft + payment.Interest;
            }

            await _connection.ExecuteAsync(sql, payment);
            return payment;
        }
        public async Task<int> AddLoan(Loan loan)
        {
            string sql = "INSERT INTO Loan (LoanAmount, StartDate, EndDate, InterestRate, DownPayment, PaymentsPerYear) " +
                "VALUES (@LoanAmount, @StartDate, @EndDate, @InterestRate, @DownPayment, @PaymentsPerYear); " +
                "SELECT LAST_INSERT_ID();";

            int newId = await _connection.QuerySingleAsync<int>(sql, loan);
            return newId;
        }

    }
}
