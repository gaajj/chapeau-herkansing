using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public void InsertPayment(Payment payment) //  baserepo gebruiken
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                INSERT INTO payments (orderId, amountPaid, paymentMethod, tip, feedback)
                VALUES (@orderId, @amountPaid, @paymentMethod, @tip, @feedback);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", payment.Order.OrderID);
                        command.Parameters.AddWithValue("@amountPaid", payment.AmountPaid);
                        command.Parameters.AddWithValue("@paymentMethod", payment.paymentMethodEnum.ToString());
                        command.Parameters.AddWithValue("@tip", payment.Tip);
                        command.Parameters.AddWithValue("@feedback", payment.Feedback ?? (object)DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();

                    }

                    string updateSql = @"
                UPDATE dbo.orderLines
                SET orderStatus = 'none'
                WHERE orderId = @orderId;
            ";
                    using (SqlCommand updateCmd = new SqlCommand(updateSql, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@orderId", payment.Order.OrderID);
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FOUT bij insert: " + ex.Message);
            }
        }
    }
}
