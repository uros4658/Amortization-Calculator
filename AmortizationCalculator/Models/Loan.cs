namespace AmortizationCalc.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int LoanAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double InterestRate { get; set; }
        public int DownPayment { get; set; }
        public int PaymentsPerYear { get; set; }
        public string Username { get; set; }
    }
}
