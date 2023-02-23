using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDesk.Core.Database;

namespace MyDesk.Data
{
    public abstract class BaseDbContext : DbContext, IContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        private void SetContextEntry<T>(T item, EntityState state = EntityState.Added) where T : class
        {
            var entry = Entry(item);
            if (entry.State == EntityState.Detached)
            {
                try
                {
                    Set<T>().Attach(item);
                }
                catch (Exception ex)
                {
                    //Debug.Print(ex.Message);
                    Set<T>().Add(item);
                }

                entry.State = state;
            }
        }

        public IQueryable<T> AsQueryable<T>() where T : class
        {
            return Set<T>().AsQueryable();
        }

        public IQueryable<T> AsNoTracking<T>() where T : class
        {
            return Set<T>().AsNoTracking();
        }

        public void Insert<T>(T item) where T : class
        {
            try
            {
                SetContextEntry(item);
                SaveChanges();
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
            }
        }

        public void Insert<T>(IEnumerable<T> items) where T : class
        {
            try
            {
                foreach (var item in items)
                {
                    SetContextEntry(item);
                }
                SaveChanges();
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
            }
        }

        public void Update<T>(IEntity<T> item) where T : class
        {
            try
            {
                SetContextEntry(item, EntityState.Modified);
                SaveChanges();
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
            }
        }

        public void UpdateAsync<T>(IEntity<T> item) where T : class
        {
            throw new NotImplementedException();
        }

        public void Modify<T>(T item) where T : class
        {
            try
            {
                SetContextEntry(item, EntityState.Modified);
                SaveChanges();
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
                throw;
            }
        }

        public void UpdateMany<T, TField>(Expression<Func<T, bool>> expression, Expression<Func<T, TField>> field, TField value) where T : class
        {
            throw new NotImplementedException();
        }

        public void UpdateMany<T>(IEnumerable<T> items) where T : class
        {
            try
            {
                foreach (var item in items)
                {
                    SetContextEntry(item, EntityState.Modified);
                }
                SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateManyAsync<T, TField>(Expression<Func<T, bool>> expression, Expression<Func<T, TField>> field, TField value) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(IEntity<T> item) where T : class
        {
            try
            {
                var set = Set<T>();
                var entity = item as T;
                set.Attach(entity);
                set.Remove(entity);
                SaveChanges();
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
            }
        }

        public void Delete<T>(Expression<Func<T, bool>> exp) where T : class
        {
            var items = Set<T>().Where(exp).ToList();
            if (!items.Any())
            {
                return;
            }
            Set<T>().RemoveRange(items);
            SaveChanges();
        }

        public IEnumerable<T> FindItems<T>(Expression<Func<T, bool>> exp) where T : class
        {
            try
            {
                return Set<T>().Where(exp).ToList();
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
                return null;
            }
        }

        public long Count<T>(Expression<Func<T, bool>> exp) where T : class
        {
            return Set<T>().Count(exp);
        }

        public T Get<T>(Expression<Func<T, bool>> exp) where T : class
        {
            try
            {
                return Set<T>().FirstOrDefault(exp);
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
                return null;
            }
        }

        public IList<T> AllItems<T>() where T : class
        {
            return Set<T>().ToList();
        }

        public void ExecuteSqlQuery(string query, IEnumerable<object> parameters = null)
        {
            try
            {
                parameters = parameters ?? new List<object>();
                Database.ExecuteSqlRaw(query, parameters);
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
                throw;
            }
        }

        public string ExecuteScalar(string query)
        {
            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                Database.OpenConnection();
                var result = command.ExecuteScalar();
                return result.ToString();
            }
        }

        object IContext.BeginTransaction()
        {
            throw new NotImplementedException();
        }
    }
}
