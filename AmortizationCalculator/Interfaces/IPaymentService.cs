namespace AmortizationCalculator.Interfaces
{
    public interface IPaymentService
    {
        public Task DeleteOtherPayments(int id);
    }

}
