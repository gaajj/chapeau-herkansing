using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Readers;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class OrderRepository : BaseRepository
    {
        public OrderRepository(IConfiguration configuration) : base(configuration) { }

        public Order GetOrderById(int orderId)
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
                    o.id = @orderId
                ORDER BY
                    ol.orderTime;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@orderId", orderId }
            };

            return ExecuteQuery(query, ReadOrderWithLines, parameters).FirstOrDefault();
        }


        public List<Order> GetAll()
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
                    o.isDeleted = 0
                ORDER BY
                    ol.orderTime;
            ";

            return ExecuteQuery(query, ReadOrderWithLines, null);
        }

        public Order GetOrderByTable(int tableId)
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

        public void AddMenuItemToOrder(Order order, MenuItem menuItem, Staff staff, int amount)
        {
            string query = @"
                INSERT INTO orderLines (orderId, menuItemId, staffId, amount, orderTime)
                VALUES (@orderId, @menuItemId, @staffId, @amount, GETDATE())";

            var parameters = new Dictionary<string, object>
            {
                { "@orderId", order.OrderID },
                { "@menuItemId", menuItem.MenuItemID },
                { "@staffId", staff.Id },
                { "@amount", amount }
            };

            ExecuteNonQuery(query, parameters);
        }

        public void UpdateOrderLineAmount(int orderLineId, int newAmount)
        {
            string query = @"
                UPDATE orderLines
                SET amount = @amount
                WHERE id = @orderLineId;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@amount", newAmount },
                { "@orderLineId", orderLineId }
            };

            ExecuteNonQuery(query, parameters);
        }

        private Order ReadOrderWithLines(SqlDataReader reader)
        {
            Order order = OrderReader.Read(reader);

            do
            {
                if (!reader.IsDBNull(reader.GetOrdinal("orderLineId")))
                {
                    OrderLine orderLine = OrderLineReader.Read(reader, order);
                    order.OrderLines.Add(orderLine);
                }
            } while (reader.Read() && reader.GetInt32(reader.GetOrdinal("orderId")) == order.OrderID);

            return order;
        }
    }
}
