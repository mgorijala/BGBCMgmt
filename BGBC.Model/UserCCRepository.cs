using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class UserCCRepository : IRepository<UserCC, int>
    {
        BGBCEntities context = new BGBCEntities();

        public IQueryable<UserCC> Get()
        {
            throw new NotImplementedException();
        }

        public List<UserCC> GetRef(int id)
        {
            throw new NotImplementedException();
        }

        public UserCC Get(int id)
        {
            return context.UserCCs.Where(x => x.UserID == id).FirstOrDefault();
        }

        public UserCC Add(UserCC entity)
        {
            entity.Createdon = DateTime.Now;
            context.UserCCs.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(UserCC entity)
        {
            throw new NotImplementedException();
        }

        public void Update(UserCC entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
