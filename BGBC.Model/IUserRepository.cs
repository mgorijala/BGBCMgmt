using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public interface IUserRepository : IRepository<User, int>
    {
        User Find(string email, string password);

        IEnumerable<User> GetOwners();
        
        User Find(string email);

    }
}
