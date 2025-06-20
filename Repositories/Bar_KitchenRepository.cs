using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Repositories.Readers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;

namespace ChapeauHerkansing.Repositories
{
    public class Bar_KitchenRepository : BaseRepository, IBar_KitchenRepository
    {
        public Bar_KitchenRepository(IConfiguration configuration) : base(configuration) { }


        public List<Order> GetAllOrdersByStatusAndCategory(OrderStatus orderStatus, Role role)
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
                    o.isDeleted = 0 and ol.orderStatus=@orderStatus and mi.category in (_categories)
                ORDER BY
                    o.orderTime;
            ";


            ApplyCategoryFilter(role, out string categoryFilter, out Dictionary<string, object> categoryParams);

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@orderStatus", orderStatus.ToString()  }

            };

            foreach (var param in categoryParams)
            {
                parameters.Add(param.Key, param.Value);
            }

            query = query.Replace("_categories", categoryFilter);


            try
            {
                return ExecuteGroupedQuery<Order>(query, MapOrderWithLines, parameters);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Kon orders niet ophalen (status={orderStatus}, rol={role}).", ex);
            }
        }

        private void ApplyCategoryFilter(Role role, out string categoryFilter, out Dictionary<string, object> categoryParams)
        {
       
          List<MenuCategory> categoriesToUse;

            if (role.Equals(Role.Chef))
            { categoriesToUse =  new List<MenuCategory> {  MenuCategory.Tussengerecht,
        MenuCategory.Hoofdgerecht,
        MenuCategory.Voorgerecht,
        MenuCategory.Nagerecht }; }
            else if (role.Equals(Role.Barman))
            { categoriesToUse = new List<MenuCategory> { MenuCategory.Dranken }; }

            else
            {
                throw new ArgumentException("Unsupported role: " + role);
            }

            categoryFilter = string.Join(", ", categoriesToUse.Select((c, i) => $"@cat{i}"));
            categoryParams = new Dictionary<string, object>();

            for (int i = 0; i < categoriesToUse.Count; i++)
            {
                string paramName = $"@cat{i}";
                categoryParams[paramName] = categoriesToUse[i].ToString();
            }
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
            try
            {
                ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
          $"Kon status orderregel #{orderLineId} niet bijwerken.", ex);
            }
        }

      

        private Order MapOrderWithLines(SqlDataReader reader, Dictionary<int, Order> dict)
        {
            int orderId = reader.GetInt32(reader.GetOrdinal("orderId"));

            if (!dict.TryGetValue(orderId, out Order order))
            {
                order = OrderReader.Read(reader);
               
                dict.Add(orderId, order);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("orderLineId")))
            {
                OrderLine line = OrderLineReader.Read(reader, order);
                order.OrderLines.Add(line);
            }

            return order;
        }


    }
}
