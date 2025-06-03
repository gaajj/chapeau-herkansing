namespace ChapeauHerkansing.Models
{
    public class StaffCollection
    {
        public List<Staff> StaffMembers { get; set; }

        public StaffCollection(List<Staff> staffMembers)
        {
            StaffMembers = staffMembers;
        }
    }
}
