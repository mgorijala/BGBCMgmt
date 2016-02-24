using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class UserRepository : IUserRepository
    {
        BGBCEntities context = new BGBCEntities();
        public User Find(string email, string password)
        {
            User user = context.Users.Where(u => u.Email == email && u.Password == password && u.Deletedon == null).FirstOrDefault();
            return user;
        }

        public IQueryable<User> Get()
        {
            throw new NotImplementedException();
        }

        public User Get(int id)
        {
            return context.Users.Include("Profiles").Include("UserReferences").Include("Tenants").Where(a => a.UserID == id).FirstOrDefault();
        }

        public User Add(User entity)
        {
            entity.Createdon = DateTime.Now;
            context.Users.Add(entity);
             
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return entity;
        }

        public void Remove(User entity)
        {
            entity.Deletedon = DateTime.Now;
            context.Entry(entity).State = EntityState.Modified;
            if (entity.Tenants.Count() > 0)
            {
                Tenant _tenant = entity.Tenants.FirstOrDefault();
                _tenant.Deletedon = DateTime.Now;
                context.Entry(_tenant).State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void Update(User entity)
        {
            try
            {
                entity.Updatedon = DateTime.Now;
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var validationErrors in e.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }


        public IEnumerable<User> GetOwners()
        {
            return context.Users.Where(u => u.UserType == 1).ToList();
        }


        public List<User> GetRef(int id)
        {
            throw new NotImplementedException();
        }


        public User Find(string email)
        {
            User user = context.Users.Where(u => u.Email == email).FirstOrDefault();
            return user;
        }
    }
}
