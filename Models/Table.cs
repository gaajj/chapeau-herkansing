namespace ChapeauHerkansing.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public User? Staff { get; set; }
        public int? Seats { get; set; }
        public bool? IsReserved { get; set; }

        // Basic Constructor
        public Table(int tableID, User? staff, int? seats, bool? isReserved)
        {
            TableID = tableID;
            Staff = staff;
            Seats = seats;
            IsReserved = isReserved;
        }
    }
}

