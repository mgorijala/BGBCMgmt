using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class PropertyRepository : IRepository<Property, int>
    {
        BGBCEntities context = new BGBCEntities();
        public IQueryable<Property> Get()
        {
            return context.Properties.Where(d => d.Deletedon == null);
        }

        public Property Get(int id)
        {
            return context.Properties.Find(id);
        }

        public Property Add(Property entity)
        {
            entity.Createdon = DateTime.Now;
            context.Properties.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Edit(Property entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void Remove(Property entity)
        {
            entity.Deletedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void Update(Property entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }


        public List<Property> GetRef(int id)
        {
            return context.Properties.Include("Tenants").Where(d => d.UserID == id && d.Deletedon == null).ToList();
        }
    }
}