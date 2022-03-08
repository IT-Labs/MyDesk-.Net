using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext context;
        private DbSet<T> entities;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.IsDeleted = true;
            entities.Update(entity);
            context.SaveChanges();
        }

        public T Get(int id)
        {
             return entities.Where(z => z.Id == id && !z.IsDeleted).FirstOrDefault();
            //return entities.SingleOrDefault(a => a.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return entities.Where(z => !z.IsDeleted).AsEnumerable();
           // return entities.AsEnumerable();
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }
    }
}
