namespace AmortizationCalculator.Models
{
    public class PaymentEachN
    {
        public Payment? paymentUsual { get; set; }
        public Payment? paymentNMonths { get; set; }
        public int nMonths { get; set; }
    }
}
