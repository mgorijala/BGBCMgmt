using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public class ZipRepository : IRepository<ZipCode, int>
    {
        BGBCEntities context = new BGBCEntities();

        public IQueryable<ZipCode> Get()
        {
            throw new NotImplementedException();

        }

        public List<ZipCode> GetRef(int id)
        {
            throw new NotImplementedException();
        }

        public ZipCode Get(int id)
        {
            return context.ZipCodes.Where(z => z.ZipCode1 == id).FirstOrDefault();

        }

        public ZipCode Add(ZipCode entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(ZipCode entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ZipCode entity)
        {
            throw new NotImplementedException();
        }
    }
}
