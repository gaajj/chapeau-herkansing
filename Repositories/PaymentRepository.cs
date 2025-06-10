using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class PaymentRepository : BaseRepository, IPaymentRepository
    {
        public PaymentRepository(IConfiguration configuration) : base(configuration) { }

        public void InsertPayment(Payment payment)
        {
            try
            {
                using (SqlConnection connection = CreateConnection()) 
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
                }
            }
            catch (Exception ex) // cw weg
            {
                Console.WriteLine("FOUT bij insert: " + ex.Message);
            }
        }
    }
}
