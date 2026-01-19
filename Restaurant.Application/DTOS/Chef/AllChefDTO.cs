using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Chef
{
    public class AllChefDTO
    {
        public int ChefId { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public int UserId { get; set; }
    }
}
