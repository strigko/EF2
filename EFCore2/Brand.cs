using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFP48.EFCore.Data.Entity
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<Product>? Products { get; set; }

        public override string ToString()
        {
            return $"ID: {Id} - Name: {Name}";
        }
    }
}
