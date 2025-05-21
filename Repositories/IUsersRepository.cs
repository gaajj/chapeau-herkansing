using ChapeauHerkansing.Models;

public interface IUsersRepository
{
    List<User> GetAll();
    User? GetByUsername(string username);

    // User? GetById(int id);
    // void Add(User user);
    // void Update(User user);
    // void Delete(int id);
}
