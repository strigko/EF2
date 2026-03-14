using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFP48.DapperCase.Entity
{
    internal class User
    {
        public Guid Id { get; set; }
        public string Name{ get; set; }
        public string Surname{ get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $@"
-------------------------------------
UserID: {Id}
Name -> {Name}
Surname -> {Surname}
Age -> {Age}
-------------------------------------
";
        }
    }
}
