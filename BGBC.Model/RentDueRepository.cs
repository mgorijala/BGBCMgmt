using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class RentDueRepository : IRepository<RentDue, int?>
    {
        BGBCEntities context = new BGBCEntities();
        public IQueryable<RentDue> Get()
        {
            throw new NotImplementedException();
        }

        public List<RentDue> GetRef(int? id)
        {
            return context.RentDues.Where(x => x.UserID == id && x.DueStatus == true).ToList(); //get only pending amount
        }

        public RentDue Get(int? id)
        {
            throw new NotImplementedException();
        }

        public RentDue Add(RentDue entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(RentDue entity)
        {
            throw new NotImplementedException();
        }

        public void Update(RentDue entity)
        {
            throw new NotImplementedException();
        }
    }
}
