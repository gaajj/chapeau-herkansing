namespace ChapeauHerkansing.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public Staff? Staff { get; set; }
        public int? Seats { get; set; }
        public string? Status { get; set; }

        public Table(int tableID, Staff? staff, int? seats, string? status)
        {
            TableID = tableID;
            Staff = staff;
            Seats = seats;
            Status = status;
        }
    }
}

