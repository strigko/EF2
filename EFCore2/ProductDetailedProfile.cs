using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFP48.EFCore.Data.Entity
{
    public class ProductDetailedProfile
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, Description: {Description}, Price: {Price}, Category: {CategoryName}, Brand: {BrandName}";
        }
    }
}
