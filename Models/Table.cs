namespace ChapeauHerkansing.Models
{
    // Table model met status enum
    public class Table
    {
        public int TableID { get; set; }
        public Staff? Staff { get; set; }
        public int? Seats { get; set; }
        public TableStatus Status { get; set; }

        // Constructor voor het aanmaken van een tafelobject
        public Table(int tableID, Staff? staff, int? seats, TableStatus status)
        {
            TableID = tableID;
            Staff = staff;
            Seats = seats;
            Status = status;
        }
    }

}

