using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

public interface IRepository<T> where T : class
{
    List<T> GetAll();
    // T GetById(int id);
    // void Add(T class);
    // void Update(T class);
    // void Delete(int id);
}
