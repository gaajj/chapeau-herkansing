using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using Microsoft.Data.SqlClient;
using System;

namespace ChapeauHerkansing.Repositories.Readers
{
    public static class MenuItemReader
    {
        public static MenuItem Read(SqlDataReader reader)
        {
            return new MenuItem(
                reader.GetInt32(reader.GetOrdinal("MenuItemID")),
                reader.GetString(reader.GetOrdinal("itemName")),
                reader.GetDecimal(reader.GetOrdinal("price")),
                ParseCategory(reader.GetString(reader.GetOrdinal("category"))),
                reader.GetBoolean(reader.GetOrdinal("isAlcoholic")),
                reader.IsDBNull(reader.GetOrdinal("isDeleted")) ? null : reader.GetBoolean(reader.GetOrdinal("isDeleted")),
                reader.GetInt32(reader.GetOrdinal("stockAmount")),
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
    }
}
