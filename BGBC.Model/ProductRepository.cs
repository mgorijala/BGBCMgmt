using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class ProductRepository : IRepository<Product, int?>
    {

        BGBCEntities context = new BGBCEntities();
        public IQueryable<Product> Get()
        {
            return context.Products.Where(d => d.Deletedon == null);
        }

        public Product Get(int? id)
        {
            return context.Products.Find(id);
        }

        public Product Add(Product entity)
        {
            entity.Createdon = DateTime.Now;
            context.Products.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Remove(Product entity)
        {
            entity.Deletedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }


        public void Update(Product entity)
        {
            entity.Updatedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }


        Product IRepository<Product, int?>.Get(int? id)
        {
            return context.Products.Find(id);
        }


        public List<Product> GetRef(int? id)
        {
            throw new NotImplementedException();
        }
    }
}
