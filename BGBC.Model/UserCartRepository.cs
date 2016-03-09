using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class UserCartRepository : IRepository<UserCart, int?>
    {
        BGBCEntities context = new BGBCEntities();
        IQueryable<UserCart> IRepository<UserCart, int?>.Get()
        {
           return context.UserCarts;
        }

        List<UserCart> IRepository<UserCart, int?>.GetRef(int? id)
        {
            throw new NotImplementedException();
        }

        UserCart IRepository<UserCart, int?>.Get(int? id)
        {
            throw new NotImplementedException();
        }

        public UserCart Add(UserCart entity)
        {
            entity.Createdon = DateTime.Now;
            context.UserCarts.Add(entity);
            context.SaveChanges();
            return entity;
        }
        void IRepository<UserCart, int?>.Remove(UserCart entity)
        {
            UserCart usercart;
            usercart = context.UserCarts.Where(x => x.ProductID == entity.ProductID && x.UserID == entity.UserID && x.Deletedon == null).FirstOrDefault();
            if (usercart != null)
            {
                usercart.Deletedon = DateTime.Now;
                context.Entry(usercart).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Update(UserCart entity)
        {
            entity.Deletedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
           
        }

    }
}
