﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class ContactRepository : IRepository<ContactForm, int>
    {
        BGBCEntities context = new BGBCEntities();

        public IQueryable<ContactForm> Get()
        {
            return context.ContactForms;
        }

        public List<ContactForm> GetRef(int id)
        {
            return context.ContactForms.Where(x => x.MessageType == id).ToList();
        }

        public ContactForm Get(int id)
        {
            throw new NotImplementedException();
        }

        public ContactForm Add(ContactForm entity)
        {
            try
            {
                entity.Createdon = DateTime.Now;
                context.ContactForms.Add(entity);
                context.SaveChanges();
                return entity;
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

        public void Remove(ContactForm entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ContactForm entity)
        {
            throw new NotImplementedException();
        }
    }
}
