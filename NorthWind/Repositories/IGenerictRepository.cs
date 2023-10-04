namespace NorthWind.Repositories
{
    public interface IGenerictRepository<T>

    {
        void Create(T entity);
        void Update(T entity);
        public IEnumerable<T> GetAll();
        T GetById(int Id);
        void Delete(T entity);
        void DeleteById(int id);
        void Save();
        IQueryable<T> FindAll();
    }
}
