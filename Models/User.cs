namespace ChapeauHerkansing.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public User(int id, string firstName, string lastName, string username, string password, string role) {
          this.Id = id;
          this.FirstName = firstName;
          this.LastName = lastName;
          this.Username = username;
          this.Password = password;
          this.Role = role;
        }
    }
}
