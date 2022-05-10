namespace Patient_Group_Service.Interfaces;


public interface IGenericRepository<T> where T : class
{
    T? GetById(string id);
    IEnumerable<T> GetAll();
    T? Add(T entity);
    void Remove(T entity);
}