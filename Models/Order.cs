using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public Table? Table { get; set; }
     
        public bool IsDeleted { get; set; }

        public DateTime Timecreated { get; set; } 



        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        public Order(int orderID, Table table, bool isDeleted, DateTime timeCreated)
        {
            OrderID = orderID;
            Table = table;

            IsDeleted = isDeleted;
            Timecreated = timeCreated;


            OrderLines = new List<OrderLine>(); // 1 van de 2 kan weg 
        }

        public Order() { }

    }
}