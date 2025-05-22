using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Models
{

    public class Staff
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }

        public Staff(int id, string firstName, string lastName, string username, string password, Role role)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Username = username;
            this.Password = password;
            this.Role = role;
        }
        public Staff(int id, string firstName, string lastName, string username, string password, Role role, bool isDeleted = false)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
            Role = role;
            IsDeleted = isDeleted;
        }
    }
}
