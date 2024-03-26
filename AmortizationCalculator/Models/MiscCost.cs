using System.ComponentModel.DataAnnotations.Schema;

namespace AmortizationCalculator.Models
{
    public class MiscCost
    {
        public int FrequencyMonths { get; set; }
        public int Cost { get; set; }
        public int LoanID { get; set; }
    }
}
