using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
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
                    o.orderTime AS orderOrderTime,
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
                    ol.orderTime AS orderLineOrderTime,
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

            return ExecuteSingle(query, ReadOrderWithLines, parameters);
        }


        public List<Order> GetAll()
        {
            string query = @"
                SELECT
                    o.id AS orderId,
                    o.isDeleted,
                   o.timeCreated,
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
                inner JOIN
                    dbo.orderLines ol ON o.id = ol.orderId
                LEFT JOIN
                    dbo.menuItems mi ON ol.menuItemId = mi.id
                LEFT JOIN
                    dbo.staff s ON ol.staffId = s.id
                WHERE
                    o.isDeleted = 0
                ORDER BY
                    o.timeCreated;
            ";

            List<Order> orders = ExecuteGroupedQuery<Order>(query, MapOrderWithLines, null);
            return orders;
        }

        public List<Order> GetAllNotReady()
        {
            string query = @"
                SELECT
                    o.id AS orderId,
                    o.isDeleted,
                   o.timeCreated,
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
                inner JOIN
                    dbo.orderLines ol ON o.id = ol.orderId
                LEFT JOIN
                    dbo.menuItems mi ON ol.menuItemId = mi.id
                LEFT JOIN
                    dbo.staff s ON ol.staffId = s.id
                WHERE
                    o.isDeleted = 0 and ol.orderStatus='BeingPrepared'
                ORDER BY
                    o.timeCreated;
            ";

            List<Order> orders = ExecuteGroupedQuery<Order>(query, MapOrderWithLines, null);
            return orders;
        }

        public List<Order> GetAllReady()
        {
            string query = @"
                SELECT
                    o.id AS orderId,
                    o.isDeleted,
                   o.timeCreated,
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
                inner JOIN
                    dbo.orderLines ol ON o.id = ol.orderId
                LEFT JOIN
                    dbo.menuItems mi ON ol.menuItemId = mi.id
                LEFT JOIN
                    dbo.staff s ON ol.staffId = s.id
                WHERE
                    o.isDeleted = 0 and ol.orderStatus='Ready'
                ORDER BY
                    o.timeCreated;
            ";

            List<Order> orders = ExecuteGroupedQuery<Order>(query, MapOrderWithLines, null);
            return orders;
        }

        public Order GetOrderByTable(int tableId)
        {
            string query = @"
                SELECT
                    o.id AS orderId,
                    o.isDeleted,
                    o.orderTime AS orderOrderTime,
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
                    ol.orderTime AS orderLineOrderTime,
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

        public void AddMenuItemToOrder(Order order, MenuItem menuItem, Staff staff, int amount, OrderStatus status)
        {
            string query = @"
                INSERT INTO orderLines (orderId, menuItemId, staffId, amount, orderTime, orderStatus)
                VALUES (@orderId, @menuItemId, @staffId, @amount, GETDATE(), @status)
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@orderId", order.OrderID },
                { "@menuItemId", menuItem.MenuItemID },
                { "@staffId", staff.Id },
                { "@amount", amount },
                { "@status", status }
            };

            ExecuteNonQuery(query, parameters);
        }

        public void ToggleOrderLineStatus(int orderLineId)
        {
            string query = @"
        UPDATE orderLines
        SET orderStatus = 
            CASE 
                WHEN orderStatus = 'Ready' THEN 'BeingPrepared'
                ELSE 'Ready'
            END
        WHERE id = @orderLineId;
    ";

            var parameters = new Dictionary<string, object>
    {
        { "@orderLineId", orderLineId }
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

        public void RemoveOrderLine(int orderLineId)
        {
            string query = @"
                DELETE FROM orderLines
                WHERE id = @orderLineId;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@orderLineId", orderLineId }
            };

            ExecuteNonQuery(query, parameters);
        }

        public void UpdateOrderLineNote(int orderLineId, string note)
        {
            string query = @"
                UPDATE orderLines
                SET note = @note
                WHERE id = @orderLineId;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@note", string.IsNullOrWhiteSpace(note) ? DBNull.Value : note },
                { "@orderLineId", orderLineId }
            };

            ExecuteNonQuery(query, parameters);
        }

        private Order MapOrderWithLines(SqlDataReader reader, Dictionary<int, Order> dict)
        {
            int orderId = reader.GetInt32(reader.GetOrdinal("orderId"));

            if (!dict.TryGetValue(orderId, out Order order))
            {
                order = OrderReader.Read(reader);
                //order.OrderLines = new List<OrderLine>();
                dict.Add(orderId, order);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("orderLineId")))
            {
                OrderLine line = OrderLineReader.Read(reader, order);
                order.OrderLines.Add(line);
            }

            return order;
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
