namespace ChapeauHerkansing.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public Staff? Staff { get; set; }
        public int? Seats { get; set; }
        public bool? IsReserved { get; set; }

        // Basic Constructor
        public Table(int tableID, Staff? staff, int? seats, bool? isReserved)
        {
            TableID = tableID;
            Staff = staff;
            Seats = seats;
            IsReserved = isReserved;
        }
    }
}

