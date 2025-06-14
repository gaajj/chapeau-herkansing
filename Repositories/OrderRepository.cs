using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Repositories.Readers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ChapeauHerkansing.Repositories
{
    public class OrderRepository : BaseRepository, IOrderRepository
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
                    s.id AS ID,
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
                    mi.isAlcoholic,
                    mi.menuType,
                    mi.stockAmount
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

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@orderId", orderId }
            };

            return ExecuteSingle(query, OrderReader.ReadWithLines, parameters);
        }


        public List<Order> GetAllOrders()
        {
            string query = @"
                SELECT
                   o.id AS orderId,
                    o.isDeleted,
                    o.orderTime AS orderOrderTime,
                    t.id AS tableId,
                    t.seats,
                    t.tableStatus,
                    s.id AS ID,
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
                    mi.isAlcoholic,
                    mi.menuType,
                    mi.stockAmount
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

            List<Order> orders = ExecuteGroupedQuery<Order>(query, MapOrderWithLines, null);
            return orders;
        }

        public List<Order> GetAllOrdersByStatus(OrderStatus orderStatus)
        {
            string query = @"
                SELECT
                    o.id AS orderId,
                    o.isDeleted,
                    o.orderTime AS orderOrderTime,
                    t.id AS tableId,
                    t.seats,
                    t.tableStatus,
                    s.id AS ID,
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
                    mi.isAlcoholic,
                    mi.menuType,
                    mi.stockAmount
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
                    o.isDeleted = 0 and ol.orderStatus=@orderStatus
                ORDER BY
                    o.orderTime;
            ";
            var parameters = new Dictionary<string, object>
            {
                { "@orderStatus", orderStatus.ToString()  }
            };


            List<Order> orders = ExecuteGroupedQuery<Order>(query, MapOrderWithLines, parameters);
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
                    s.id AS ID,
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
                    mi.isAlcoholic,
                    mi.menuType,
                    mi.stockAmount
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

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@tableId", tableId }
            };

            return ExecuteGroupedQuery<Order>(query, MapOrderWithLines, parameters).FirstOrDefault();
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
                { "@status", status.ToString() }
            };

            ExecuteNonQuery(query, parameters);
        }

        public void ToggleOrderLineStatus(int orderLineId)
        {
            //zorgt ervoor dat orders die op ready of (iets anders) teruggaan naar ordered op het moment dat de methode wordt aangeroepen
            string query = @"
        UPDATE orderLines
        SET orderStatus = 
            CASE 
                WHEN orderStatus = 'Ordered' THEN 'Ready'
                ELSE 'Ordered'
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

            Dictionary<string, object> parameters = new Dictionary<string, object>
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

            Dictionary<string, object> parameters = new Dictionary<string, object>
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

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@note", string.IsNullOrWhiteSpace(note) ? DBNull.Value : note },
                { "@orderLineId", orderLineId }
            };

            ExecuteNonQuery(query, parameters);
        }

        public void CreateOrderForTable(int tableId)
        {
            string query = @"
                INSERT INTO orders (tableId)
                VALUES (@tableId);
            ";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@tableId", tableId }
            };

            ExecuteSingle(query, OrderReader.Read, parameters);
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

        public void SoftDeleteOrder(int orderId)
        {
            string query = "UPDATE orders SET isDeleted = 1 WHERE id = @orderId;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@orderId", orderId }
            };

            ExecuteNonQuery(query, parameters);
        }

    }
}
