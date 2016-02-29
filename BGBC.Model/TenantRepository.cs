using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BGBC.Model
{
    public class TenantRepository : IRepository<Tenant, int>
    {
        BGBCEntities context = new BGBCEntities();
        public IQueryable<Tenant> Get()
        {
            return context.Tenants.Include("User").Where(x => x.Deletedon == null);
        }

        public Tenant Get(int id)
        {
            return context.Tenants.Find(id);
        }

        public Tenant Add(Tenant entity)
        {
            entity.Createdon = DateTime.Now;
            context.Tenants.Add(entity);
            context.SaveChanges();
            return entity;

        }

        public void Remove(Tenant entity)
        {
            entity.Deletedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }


        public void Update(Tenant entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }


        public List<Tenant> GetRef(int id)
        {
            return context.Tenants.Include("User").Where(x => x.PropertyID == id && x.Deletedon == null).ToList();
        }
    }
}
