using System.Collections.Generic;

namespace Mule
{
    /// <summary>
    /// Repository interface for data access
    /// Use this interface for dependency injection to mock or abstract persistence objects during testing
    /// </summary>
    /// <typeparam name="T">Repository model</typeparam>
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> Read();
        T Create(T item);
        T Update(T existing, T item = null);
        int Delete(T item);
    }

}
