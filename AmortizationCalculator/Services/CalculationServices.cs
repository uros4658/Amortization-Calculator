using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using AmortizationCalculator.Models;
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
        public async Task<int> getLastLoanID()
        {
            string sql = "SELECT MAX(ID) FROM loan";
            var maxLoanId = await _connection.QueryAsync<int>(sql);
            return maxLoanId.Single();
        }
        public async Task<Payment> AddMiscCost(Payment payment, MiscCost misccost)
        {
            payment.MonthlyPayment += misccost.Cost;
            payment.Interest += misccost.Cost;
            return payment;
        }

        public async Task<Payment> RegisterOneMonth(Loan loan, Payment payment, MiscCost[] miscCost)
        {
            string sql = "INSERT INTO payment (amountleft, monthlypayment, principal, interest, loanmonth, loanID) " +
                "VALUES (@amountleft, @monthlypayment, @principal, @interest, @loanmonth, @loanID)";

            payment = await AdjustPayment(payment, loan);

            if (payment.AmountLeft + payment.Interest < payment.MonthlyPayment)
            {
                payment.MonthlyPayment = payment.AmountLeft + payment.Interest;
                payment.Principal = payment.MonthlyPayment - payment.Interest;
            }
            // place holders
            double monthlyPayment = payment.MonthlyPayment;
            double interestPayment = payment.Interest;

            int monthsSinceStart = ((payment.LoanMonth.Year - loan.StartDate.Year) * 12) + payment.LoanMonth.Month - loan.StartDate.Month;

            for (int i = 0; i < miscCost.Length; i++)
            {
                if (monthsSinceStart % miscCost[i].FrequencyMonths == 0)
                {
                    payment = await AddMiscCost(payment, miscCost[i]);
                }
            }

            await _connection.ExecuteAsync(sql, payment);

            payment.MonthlyPayment = monthlyPayment;
            payment.Interest = interestPayment;

            return payment;
        }

        public async Task<IEnumerable<Loan>> GetAllLoans(string username)
        {
            string sql = "SELECT * FROM loan WHERE username = @username;";
            return await _connection.QueryAsync<Loan>(sql, new { username });
        }
        public async Task<int> AddLoan(Loan loan)
        {
            string sql = "INSERT INTO Loan (LoanAmount, StartDate, EndDate, InterestRate, DownPayment, PaymentsPerYear, Username) " +
                "VALUES (@LoanAmount, @StartDate, @EndDate, @InterestRate, @DownPayment, @PaymentsPerYear, @Username); " +
                "SELECT LAST_INSERT_ID();";

            int newId = await _connection.QuerySingleAsync<int>(sql, loan);
            return newId;
        }

        public async Task<MiscCost[]> GetMisc(int LoanID)
        {
            string sql = "SELECT Cost, LoanID, frequency as FrequencyMonths FROM miscCost WHERE LoanID = @LoanID;";
            var miscCosts = await _connection.QueryAsync<MiscCost>(sql, new { LoanID });
            return miscCosts.ToArray();
        }

        public async Task InsertMisc(MiscCost miscCost)
        {
            string sql = "INSERT INTO miscCost (frequency, cost, LoanID) VALUES (@Frequency, @Cost, @LoanID);";

            var parameters = new DynamicParameters();
            parameters.Add("Frequency", miscCost.FrequencyMonths);
            parameters.Add("Cost", miscCost.Cost);
            parameters.Add("LoanID", miscCost.LoanID);

            await _connection.ExecuteAsync(sql, parameters);
        }
    }
}
