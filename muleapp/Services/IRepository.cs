using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mule
{
    public class RepositoryContext : DbContext
    {
        public IEnumerable<Type> ModelTypes { get; private set; }
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) {
            ModelTypes = ModelService.AllModels();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach(var t in ModelTypes) //Add all model classes to database context
            {
                modelBuilder.Entity(t)
                .ToTable(t.FullName);
            }
        }
    }

    /// <summary>
    /// Repository interface for data access
    /// Use this interface for dependency injection to mock or abstract persistence objects during testing
    /// </summary>
    /// <typeparam name="T">Repository model</typeparam>
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Read();
        T Create(T item);
        T Update(T existing, T item = null);
        int Delete(T item);
    }


    /// <summary>
    /// Access to SQLite repository from SQLiteContext
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        readonly RepositoryContext _context;
        public Repository(RepositoryContext context)
        {
            _context = context;
            try
            {
                context.Database.EnsureCreated();
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("The process has no package identity"))
            {
                //Ignore this - sqlite needs signature
            }
        }

        public T Create(T item)
        {
            _context.Add(item);
            _context.SaveChanges();
            return item;
        }

        public IEnumerable<T> Read()
        {
            return _context.Set<T>().AsEnumerable();
        }

        public T Update(T existing, T item = null)
        {
            if (item != null)
            {
                var vals = ModelService.GetModelValues(item);
                foreach (var prop in ModelService.GetPropertyInfo(typeof(T)))
                {
                    var newprop = vals.Where(x => x.Key == prop.Name).FirstOrDefault().Value;
                    prop.SetValue(existing, newprop);
                }
            }
            _context.Update(existing);
            _context.SaveChanges();
            return existing;
        }

        public int Delete(T item)
        {
            _context.Remove(item);
            return _context.SaveChanges();
        }
    }
}
