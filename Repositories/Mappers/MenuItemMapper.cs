using System;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Repositories.Mappers
{
    public static class MenuItemMapper
    {
        public static MenuItem FromReader(SqlDataReader reader)
        {
            return new MenuItem
            (
                reader.GetInt32(reader.GetOrdinal("MenuItemID")),
                reader.GetString(reader.GetOrdinal("itemName")),
                reader.GetDecimal(reader.GetOrdinal("price")),
                ParseCategory(reader.GetString(reader.GetOrdinal("category"))),
                reader.GetBoolean(reader.GetOrdinal("isAlcoholic")),
                reader.IsDBNull(reader.GetOrdinal("isDeleted")) ? null : reader.GetBoolean(reader.GetOrdinal("isDeleted")),
                reader.GetInt32(reader.GetOrdinal("stockAmount")), // niet nullable
                (MenuType)reader.GetInt32(reader.GetOrdinal("menuType"))
            );
        }

        private static MenuCategory ParseCategory(string category)
        {
            return category.ToLower() switch
            {
                "voorgerecht" => MenuCategory.Voorgerecht,
                "tussengerecht" => MenuCategory.Tussengerecht,
                "hoofdgerecht" => MenuCategory.Hoofdgerecht,
                "nagerecht" => MenuCategory.Nagerecht,
                "dranken" => MenuCategory.Dranken,
                _ => throw new InvalidCastException($"Onbekende categorie: {category}")
            };
        }

        private static MenuType ParseMenuType(string menuType)
        {
            return menuType.ToLower() switch
            {
                "lunch" => MenuType.Lunch,
                "dinner" => MenuType.Dinner,
                "drinks" => MenuType.Drinks,
                _ => throw new InvalidCastException($"Onbekende menuType: {menuType}")
            };
        }
    }
}
