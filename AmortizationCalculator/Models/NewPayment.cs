namespace AmortizationCalculator.Models
{
    public class NewPayment
    {
        public int LoanID { get; set; }
        public DateTime LoanMonth { get; set; }
        public double PayAmount { get; set; }

    }
}
