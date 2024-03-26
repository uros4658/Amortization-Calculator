using AmortizationCalc.Models;
using AmortizationCalculator.Interfaces;
using AmortizationCalculator.Models;
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
            var loanId = await _connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = payment.paymentID });

            var sqlLoan = "SELECT * FROM Loan WHERE Id = @LoanId";
            var loan = await _connection.QueryFirstOrDefaultAsync<Loan>(sqlLoan, new { LoanId = loanId });

            return loan;
        }
        public async Task<int> LastLoanInserted()
        {
            string sql = "SELECT LoanID FROM payment ORDER BY paymentID DESC LIMIT 1;";
            var loanId = await _connection.QuerySingleAsync<int>(sql);
            return loanId;
        }

        public async Task<Payment> MakeDifferentPayment(Payment payment) 
        {
            string sql = "UPDATE payment SET amountleft = @amountleft, monthlypayment = @monthlypayment, principal = @principal," +
                " interest = @interest, loanmonth = @loanmonth, loanID = @loanID WHERE paymentId = @paymentID";

            Loan loan = await RetrieveLoanFromPayment(payment);

            payment = await AdjustPayment(payment);

            await _connection.ExecuteAsync(sql, payment);
            return payment;
        }
        public async Task<IEnumerable<Payment>> GetAllPayments(int loanID)
        {
            string sql = "SELECT * FROM payment WHERE loanID = @LoanId;";
            return await _connection.QueryAsync<Payment>(sql, new { LoanId = loanID });
        }

        public async Task DeleteOtherPayments(int id)
        {
            var sql = "SELECT loanID FROM payment WHERE paymentId = @id";
            var loanId = await _connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });

            // Delete all records from the payment table that have the same loanID and a paymentId greater than the given id
            sql = "DELETE FROM payment WHERE loanID = @LoanId AND paymentId > @id";
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
        // It was either this or add a new interface in the controller (less of 2 evils)
        public async Task<Payment> AddMiscCost(Payment payment, MiscCost misccost)
        {
            payment.MonthlyPayment += misccost.Cost;
            payment.Interest += misccost.Cost;
            return payment;
        }
        public async Task<MiscCost[]> GetMisc(int LoanID)
        {
            string sql = "SELECT Cost, LoanID, frequency as FrequencyMonths FROM miscCost WHERE LoanID = @LoanID;";
            var miscCosts = await _connection.QueryAsync<MiscCost>(sql, new { LoanID });
            return miscCosts.ToArray();
        }

        public async Task<Payment> RegisterAdjustedPayment(Payment payment, MiscCost[] miscCost)
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

            // place holders
            double monthlyPayment = payment.MonthlyPayment;
            double interestPayment = payment.Interest;

            int monthsSinceStart = ((payment.LoanMonth.Year - loan.StartDate.Year) * 12) + payment.LoanMonth.Month - loan.StartDate.Month;

            for (int i = 0; i < miscCost.Length; i++)
            {
                if (monthsSinceStart % miscCost[i].FrequencyMonths == 0)
                {
                    await AddMiscCost(payment, miscCost[i]);
                }
            }

            await _connection.ExecuteAsync(sql, payment);

            payment.MonthlyPayment = monthlyPayment;
            payment.Interest = interestPayment;
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
                "principal = @principal, interest = @interest, loanmonth = @loanmonth, loanID = @loanID WHERE paymentID = @paymentID";

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
        public async Task<Payment> CreatePaymentFromNewPayment(NewPayment newPayment)
        {
            string sql = "SELECT * FROM payment WHERE LoanID = @LoanID AND LoanMonth = @LoanMonth";
            var parameters = new { LoanID = newPayment.LoanID, LoanMonth = newPayment.LoanMonth };
            Payment payment = await _connection.QuerySingleOrDefaultAsync<Payment>(sql, parameters);

            // If no matching record was found, create a new one
            if (payment == null)
            {
                payment = new Payment
                {
                    LoanMonth = newPayment.LoanMonth,
                    MonthlyPayment = newPayment.PayAmount
                };
            }
            else
            {
                payment.MonthlyPayment = newPayment.PayAmount;
            }

            return payment;
        }


    }

}
