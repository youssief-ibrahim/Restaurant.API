using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public Cart Cart { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
