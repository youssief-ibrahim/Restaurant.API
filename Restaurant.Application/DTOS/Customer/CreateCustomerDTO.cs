using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Customer
{
    public class CreateCustomerDTO
    {
        public string Name { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
    }
}
