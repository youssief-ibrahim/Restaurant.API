using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Customer
{
    public class AllCustomerDTO
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}
