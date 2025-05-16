using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class OrderRepository : IRepository<Order>
    {

        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public List<Order> GetAll()
        {
            List<Order> orders = new List<Order>();
            Dictionary<int, Order> orderDictionary = new Dictionary<int, Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT orders.id as orderId, orders.tableId, orders.isdeleted, orderLines.id as orderlineId, orderLines.amount, orderLines.ordertime, orderLines.note,orderlines.orderStatus, menuItems.id as menuItemId,  menuItems.itemName FROM dbo.orders LEFT JOIN  dbo.orderLines ON orders.id = orderLines.orderId LEFT JOIN dbo.menuItems ON orderLines.menuItemId = menuItems.id order by orderLines.ordertime;";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    int orderId = reader.GetInt32(0);
                    if (!orderDictionary.ContainsKey(orderId))
                    {
                        Order order = ReadOrder(reader);
                        orderDictionary.Add(orderId, order);
                    }

                    if (!reader.IsDBNull(4))
                    { orderDictionary[orderId].OrderLines.Add(ReadOrderLine(reader, orderDictionary[orderId])); }
                }
                orders = orderDictionary.Values.ToList();
                foreach (Order order in orders)
                {
                    order.OrderLines = order.OrderLines.OrderBy(ol => ol.OrderTime).ToList();
                }
            }
                return orders;
           }
        private Order ReadOrder(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            Table table = new Table(reader.GetInt32(1), null, null, null);
            bool isdeleted = reader.GetBoolean(2);


            return new Order(id, table, isdeleted);
        }

        private OrderLine ReadOrderLine(SqlDataReader reader, Order order)
        {
            int orderLineId = reader.GetInt32(3);
            int amount = reader.GetInt32(4);
            DateTime orderTime = reader.GetDateTime(5);
            string note = reader.IsDBNull(6) ? null : reader.GetString(6);
            string orderStatus = reader.GetString(7);
            MenuItem menuItem = new MenuItem(reader.GetInt32(8), reader.GetString(9));
            
            return new OrderLine(orderLineId, order, menuItem, null, amount, orderTime, note);
        }
    } }
