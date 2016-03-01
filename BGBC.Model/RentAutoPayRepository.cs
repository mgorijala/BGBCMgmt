using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class RentAutoPayRepository : IRepository<RentAutoPay, int>
    {

        BGBCEntities context = new BGBCEntities();

        public IQueryable<RentAutoPay> Get()
        {
            return context.RentAutoPays;
        }

        public List<RentAutoPay> GetRef(int id)
        {
            throw new NotImplementedException();
        }

        public RentAutoPay Get(int id)
        {
            return context.RentAutoPays.Where(x => x.UserID == id).FirstOrDefault();
        }

        public RentAutoPay Add(RentAutoPay entity)
        {
            entity.Createdon = DateTime.Now;
            context.RentAutoPays.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(RentAutoPay entity)
        {
            try
            {
                context.RentAutoPays.Remove(entity);
                context.Entry(entity).State = EntityState.Deleted;
                context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void Update(RentAutoPay entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
