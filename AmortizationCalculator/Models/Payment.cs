public class Payment
{
    public int paymentID { get; set; }
    public DateTime LoanMonth { get; set; }
    public double MonthlyPayment { get; set; } = 0;
    public double AmountLeft { get; set; }
    public double Principal { get; set; }
    public double Interest { get; set; }
    public int LoanID { get; set; }
}