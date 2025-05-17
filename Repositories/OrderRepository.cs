using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Readers;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class OrderRepository : BaseRepository
    {

        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration) : base(configuration) { }

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
                        Order order = OrderReader.Read(reader);
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

        public Order? GetOrderByTable(int tableId)
        {
            string query = @"
                SELECT
                    o.id AS orderId,
                    o.isDeleted,
                    t.id AS tableId,
                    t.seats,
                    t.tableStatus,
                    s.id AS staffId,
                    s.firstName,
                    s.lastName,
                    s.username,
                    s.password,
                    s.role,
                    ol.id AS orderLineId,
                    ol.amount,
                    ol.orderTime,
                    ol.note,
                    ol.orderStatus,
                    mi.id AS menuItemId,
                    mi.itemName,
                    mi.price,
                    mi.category,
                    mi.isAlcoholic
                FROM
                    dbo.orders o
                INNER JOIN
                    dbo.tables t ON o.tableId = t.id
                LEFT JOIN
                    dbo.orderLines ol ON o.id = ol.orderId
                LEFT JOIN
                    dbo.menuItems mi ON ol.menuItemId = mi.id
                LEFT JOIN
                    dbo.staff s ON ol.staffId = s.id
                WHERE
                    o.tableId = @tableId AND o.isDeleted = 0
                ORDER BY
                    ol.orderTime;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@tableId", tableId }
            };

            return ExecuteQuery(query, ReadOrderWithLines, parameters).FirstOrDefault();
        }

        

        private OrderLine ReadOrderLine(SqlDataReader reader, Order order)
        {
            return new OrderLine(
                reader.GetInt32(reader.GetOrdinal("orderLineId")),
                order,
                MenuItemReader.Read(reader),
                StaffReader.Read(reader),
                reader.GetInt32(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("orderTime")),
                reader.IsDBNull(reader.GetOrdinal("note")) ? null : reader.GetString(reader.GetOrdinal("note"))
            );
        }

        private Order ReadOrderWithLines(SqlDataReader reader)
        {
            Order order = OrderReader.Read(reader);

            do
            {
                if (!reader.IsDBNull(reader.GetOrdinal("orderLineId")))
                {
                    OrderLine orderLine = ReadOrderLine(reader, order);
                    order.OrderLines.Add(orderLine);
                }
            } while (reader.Read() && reader.GetInt32(reader.GetOrdinal("orderId")) == order.OrderID);

            return order;
        }
    } }
