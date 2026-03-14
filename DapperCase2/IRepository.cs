using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFP48.DapperCase
{
    internal interface IRepository<T>
    {
        void Create (T entity);  
        void Update (T entity);  
        void Delete (Guid id);
        T GetById(Guid id);
        IEnumerable<T> GetAll();
    }
}
