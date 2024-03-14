using AmortizationCalc.Models;
using AmortizationCalculator.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace AmortizationCalculator.Services
{
    public class PaymentServices  : IPaymentService
    {
        private MySqlConnection _connection;
        private readonly IConfiguration _config;

        public PaymentServices(IConfiguration config)
        {
            _config = config;
            _connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
        }
        public async Task<Loan> RetrieveLoanFromPayment(Payment payment)
        {
            var sql = "SELECT loanID FROM payment WHERE paymentId = @Id";
            var loanId = await _connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = payment.Id });

            var sqlLoan = "SELECT * FROM Loan WHERE Id = @LoanId";
            var loan = await _connection.QueryFirstOrDefaultAsync<Loan>(sqlLoan, new { LoanId = loanId });

            return loan;
        }

        public async Task<Payment> MakeDifferentPayment(Payment payment) 
        {
            string sql = "UPDATE payment SET amountleft = @amountleft, monthlypayment = @monthlypayment, principal = @principal," +
                " interest = @interest, loanmonth = @loanmonth, loanID = @loanID WHERE paymentId = @Id";

            Loan loan = await RetrieveLoanFromPayment(payment);

            payment = await AdjustPayment(payment);

            await _connection.ExecuteAsync(sql, payment);
            return payment;
        }
        public async Task DeleteOtherPayments(int id)
        {
            var sql = "SELECT loanID FROM payment WHERE paymentId = @Id";
            var loanId = await _connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });

            // Delete all records from the payment table that have the same loanID and a paymentId greater than the given id
            sql = "DELETE FROM payment WHERE loanID = @LoanId AND paymentId > @Id";
            await _connection.ExecuteAsync(sql, new { LoanId = loanId, Id = id });
        }
        public async Task<Payment> AdjustPayment(Payment payment)
        {
            Loan loan = await RetrieveLoanFromPayment(payment);

            payment.Interest = (payment.AmountLeft * loan.InterestRate) / 12;
            payment.Principal = payment.MonthlyPayment - payment.Interest;
            payment.AmountLeft -= payment.Principal;
            if(payment.AmountLeft < 0)
            {
                payment.Principal += payment.AmountLeft;
                payment.AmountLeft = 0;
            }

            return payment;
        }
        public async Task<double> CalculateMonthlyCost(Payment payment)
        {
            Loan loan = await RetrieveLoanFromPayment(payment);

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
        public async Task<Payment> RegisterAdjustedPayment(Payment payment)
        {
            string sql = "INSERT INTO payment (amountleft, monthlypayment, principal, interest, loanmonth, loanID) " +
                "VALUES (@amountleft, @monthlypayment, @principal, @interest, @loanmonth, @loanID)";

            Loan loan = await RetrieveLoanFromPayment(payment);
            payment = await AdjustPayment(payment);

            payment.LoanMonth = payment.LoanMonth.AddMonths(12 / loan.PaymentsPerYear);

            if (payment.AmountLeft + payment.Interest < payment.MonthlyPayment)
            {
                payment.MonthlyPayment = payment.AmountLeft + payment.Interest;
            }

            await _connection.ExecuteAsync(sql, payment);
            return payment;
        }
        public async Task<Payment> RegisterAdjustedPaymentInterestDouble(Payment payment)
        {
            string sql = "INSERT INTO payment (amountleft, monthlypayment, principal, interest, loanmonth, loanID) " +
                "VALUES (@amountleft, @monthlypayment, @principal, @interest, @loanmonth, @loanID)";

            Loan loan = await RetrieveLoanFromPayment(payment);
            payment = await AdjustPayment(payment);
            payment.Interest *= 2;

            payment.LoanMonth = payment.LoanMonth.AddMonths(12 / loan.PaymentsPerYear);

            await _connection.ExecuteAsync(sql, payment);
            return payment;
        }

        public async Task<Payment> MissedPaymentRegister(Payment payment)
        {
            string sql = "UPDATE payment SET amountleft = @amountleft, monthlypayment = @monthlypayment, " +
                "principal = @principal, interest = @interest, loanmonth = @loanmonth, loanID = @loanID WHERE paymentID = @ID";

            Loan loan = await RetrieveLoanFromPayment(payment);
            double prevMonthlyPayment = payment.MonthlyPayment;

            payment.MonthlyPayment = 0;
            payment.Interest = 0;
            payment.Principal = 0;

            payment.LoanMonth = payment.LoanMonth.AddMonths(12 / loan.PaymentsPerYear);

            await _connection.ExecuteAsync(sql, payment);
            payment.MonthlyPayment = prevMonthlyPayment;

            return payment;
        }

    }

}
