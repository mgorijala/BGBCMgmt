using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class UserReferenceRepository : IRepository<UserReference, int>
    {
        BGBCEntities context = new BGBCEntities();

        public IQueryable<UserReference> Get()
        {
            throw new NotImplementedException();
        }

        public List<UserReference> GetRef(int id)
        {
            return context.UserReferences.Where(a => a.UserID == id).ToList();
        }

        public UserReference Get(int id)
        {
            throw new NotImplementedException();
        }

        public UserReference Add(UserReference entity)
        {
            entity.Createdon = DateTime.Now;
            context.UserReferences.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(UserReference entity)
        {
            throw new NotImplementedException();
        }

        public void Update(UserReference entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }


    }
}
