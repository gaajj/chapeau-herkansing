namespace ChapeauHerkansing.Repositories.Readers
{
   
    using global::ChapeauHerkansing.Models;
    using global::ChapeauHerkansing.Models.Enums;
    using Microsoft.Data.SqlClient;

    namespace ChapeauHerkansing.Repositories.Readers
    {
        public static class MenuItemReader
        {
            public static MenuItem Read(SqlDataReader reader)
            {
                return new MenuItem(
                    reader.GetInt32(reader.GetOrdinal("menuItemId")),
                    reader.GetString(reader.GetOrdinal("itemName")),
                    reader.GetDecimal(reader.GetOrdinal("price")), 
                    ParseCategory(reader.GetString(reader.GetOrdinal("category"))),
                    reader.GetBoolean(reader.GetOrdinal("isAlcoholic"))
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
}