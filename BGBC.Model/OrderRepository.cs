using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
   public  class OrderRepository:IRepository<Order,int>
    {
       BGBCEntities context = new BGBCEntities();
        public IQueryable<Order> Get()
        {
            throw new NotImplementedException();
        }

        public List<Order> GetRef(int id)
        {
            throw new NotImplementedException();
        }

        public Order Get(int id)
        {
            return context.Orders.Find(id);
        }

        public Order Add(Order entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Order entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Order entity)
        {
            throw new NotImplementedException();
        }
    }
}
