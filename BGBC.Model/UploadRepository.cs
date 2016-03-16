using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class UploadRepository : IRepository<LeaseFile, int>
    {
        BGBCEntities context = new BGBCEntities();

        public LeaseFile Get(int? id)
        {
            return context.LeaseFiles.Find(id);
        }
        public IQueryable<LeaseFile> Get()
        {
            return context.LeaseFiles.Include("Tenant");
        }

        public List<LeaseFile> GetRef(int id)
        {
            throw new NotImplementedException();
        }

        public LeaseFile Get(int id)
        {
            return context.LeaseFiles.Find(id);
        }

        public LeaseFile Add(LeaseFile entity)
        {
            entity.Createdon = DateTime.Now;
            context.LeaseFiles.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(LeaseFile entity)
        {  
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void Update(LeaseFile entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
