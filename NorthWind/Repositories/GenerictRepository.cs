using Microsoft.EntityFrameworkCore;
using NorthWind.Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using NorthWind.Models;

namespace NorthWind.Repositories
{
    public class GenerictRepository<T> : IGenerictRepository<T> where T : class
    {
        private readonly ILoggerManager _logger;
        private readonly testContext _context;

        public GenerictRepository(ILoggerManager logger, testContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Create(T entity)
        {
            _logger.LogInfo("GenerictRepository: Create");
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _logger.LogInfo("GenerictRepository: Delete");
            _context.Set<T>().Remove(entity);
        }

        public void DeleteById(int id)
        {
            _logger.LogInfo("GenerictRepository: DeleteById");
            T element = _context.Set<T>().Find(id);
            _context.Set<T>().Remove(element);
            this.Save();
        }

        public IQueryable<T> FindAll()
        {
            _logger.LogInfo("GenerictRepository: FindAll");
            return _context.Set<T>().AsQueryable();
        }

        public IEnumerable<T> GetAll()
        {
            _logger.LogInfo("GenerictRepository: GetAll");
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            _logger.LogInfo("GenerictRepository: GetById");
            return _context.Set<T>().Find(id);
        }

        public void Update(T entity)
        {
            _logger.LogInfo("GenerictRepository: Update");
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Save()
        {
            _logger.LogInfo("GenerictRepository: Save");

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database error: {ex.Message}");
                throw;
            }
        }
    }
}
