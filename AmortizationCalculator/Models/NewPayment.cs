namespace AmortizationCalculator.Models
{
    public class NewPayment
    {
        public int Id { get; set; }
        public DateTime LoanMonth { get; set; }
        public double AmountLeft { get; set; }

    }
}
