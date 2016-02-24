using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class RentPaymentRepository : IRepository<RentPayment, int>
    {
        BGBCEntities context = new BGBCEntities();
        public IQueryable<RentPayment> Get()
        {
            throw new NotImplementedException();
        }

        public List<RentPayment> GetRef(int id)
        {
            throw new NotImplementedException();
        }

        public RentPayment Get(int id)
        {
            throw new NotImplementedException();
        }

        public RentPayment Add(RentPayment entity)
        {
            entity.Createdon = DateTime.Now;
            context.RentPayments.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(RentPayment entity)
        {
            throw new NotImplementedException();
        }

        public void Update(RentPayment entity)
        {
            throw new NotImplementedException();
        }
    }
}
