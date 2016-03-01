using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class ProfileRepository : IRepository<Profile, int>
    {
        BGBCEntities context = new BGBCEntities();
        public IQueryable<Profile> Get()
        {
            return context.Profiles;
        }

        public Profile Get(int id)
        {
            return context.Profiles.Where(a => a.UserID == id).FirstOrDefault();
        }

        public Profile Add(Profile entity)
        {
            entity.Createdon = DateTime.Now;
            context.Profiles.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(Profile entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Profile entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }


        public List<Profile> GetRef(int id)
        {
            return context.Profiles.Include("User").Where(x => x.ProfileID == id).ToList();
        }
    }
}
