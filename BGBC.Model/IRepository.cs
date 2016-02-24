using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public interface IRepository<TEnt, in TPk> where TEnt : class
    {
        IQueryable<TEnt> Get();
        List<TEnt> GetRef(TPk id);
        TEnt Get(TPk id);
        TEnt Add(TEnt entity);
        void Remove(TEnt entity);
        void Update(TEnt entity);


    }
}
