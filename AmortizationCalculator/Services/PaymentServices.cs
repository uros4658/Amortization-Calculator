using AmortizationCalculator.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;

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

        public async Task DeleteOtherPayments(int id)
        {
            string sql = @"
                DELETE FROM Payment
                WHERE paymentID >= @Id AND
                (paymentID = @Id OR
                (SELECT amountLeft FROM Payment WHERE paymentID = @Id + 1) <= 0);
            ";

            await _connection.ExecuteAsync(sql, new { Id = id });
        }

    }

}
