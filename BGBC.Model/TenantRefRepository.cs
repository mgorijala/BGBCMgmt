using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class TenantRefRepository : IRepository<TenantReferral, int?>
    {
        BGBCEntities context = new BGBCEntities();
        public IQueryable<TenantReferral> Get()
        {
            return (from c in context.TenantReferrals select c);
        }

        public List<TenantReferral> GetRef(int? id)
        {
            return context.TenantReferrals.Where(e => e.ID == id).ToList();
        }

        public TenantReferral Get(int? id)
        {
            throw new NotImplementedException();
        }

        public TenantReferral Add(TenantReferral entity)
        {
            entity.Createdon = DateTime.Now;
            context.TenantReferrals.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(TenantReferral entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TenantReferral entity)
        {
            throw new NotImplementedException();
        }
    }
}
