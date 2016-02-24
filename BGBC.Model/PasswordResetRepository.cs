using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class PasswordResetRepository : IRepository<PasswordReset, int?>
    {
        BGBCEntities context = new BGBCEntities();
        IQueryable<PasswordReset> IRepository<PasswordReset, int?>.Get()
        {
            return context.PasswordResets;
        }

        List<PasswordReset> IRepository<PasswordReset, int?>.GetRef(int? id)
        {
            throw new NotImplementedException();
        }

        PasswordReset IRepository<PasswordReset, int?>.Get(int? id)
        {
            throw new NotImplementedException();
        }

        public PasswordReset Add(PasswordReset entity)
        {
            entity.CreatedDate = DateTime.Now;
            context.PasswordResets.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(PasswordReset entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }

        public void Update(PasswordReset entity)
        {
            throw new NotImplementedException();
        }


    }
}
