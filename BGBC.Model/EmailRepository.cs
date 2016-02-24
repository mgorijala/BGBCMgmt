using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class EmailRepository : IRepository<Email, int?>
    {
        BGBCEntities context = new BGBCEntities();

        IQueryable<Email> IRepository<Email, int?>.Get()
        {
            return context.Emails;
        }

        List<Email> IRepository<Email, int?>.GetRef(int? id)
        {
            throw new NotImplementedException();
        }

        Email IRepository<Email, int?>.Get(int? id)
        {
            throw new NotImplementedException();
        }

        public Email Add(Email entity)
        {
            entity.CreatedDate = DateTime.Now;
            entity.Active = true;
            context.Emails.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(Email entity)
        {
            try
            {
                context.Emails.Remove(entity);
                context.Entry(entity).State = EntityState.Deleted;
                context.SaveChanges();
            }
            catch (Exception ex)
            {


            }
        }

        public void Update(Email entity)
        {
            entity.ModifiedDate = DateTime.Now;
            context.SaveChanges();
        }
    }
}
