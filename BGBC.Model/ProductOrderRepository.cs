using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class ProductOrderRepository : IRepository<ProductOrder, int>
    {
        BGBCEntities context = new BGBCEntities();
        IQueryable<ProductOrder> IRepository<ProductOrder, int>.Get()
        {
            throw new NotImplementedException();
        }

        List<ProductOrder> IRepository<ProductOrder, int>.GetRef(int id)
        {
            throw new NotImplementedException();
        }

        ProductOrder IRepository<ProductOrder, int>.Get(int id)
        {
            throw new NotImplementedException();
        }

        public ProductOrder Add(ProductOrder entity)
        {
            context.ProductOrders.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(ProductOrder entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ProductOrder entity)
        {
            throw new NotImplementedException();
        }
    }
}
