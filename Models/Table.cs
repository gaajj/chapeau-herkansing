namespace ChapeauHerkansing.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public int? StaffID { get; set; }
        public int? Seats { get; set; }
        public bool? IsReserved { get; set; }

        // Basic Constructor
        public Table(int tableID, int? staffID, int? seats, bool? isReserved)
        {
            TableID = tableID;
            StaffID = staffID;
            Seats = seats;
            IsReserved = isReserved;
        }
    }
}

