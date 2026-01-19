using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Chef
{
    public class CreateChefDTO
    {
        public string Name { get; set; }
        public string Specialty { get; set; }
        [JsonIgnore]       
        public int UserId { get; set; }
    }
}
