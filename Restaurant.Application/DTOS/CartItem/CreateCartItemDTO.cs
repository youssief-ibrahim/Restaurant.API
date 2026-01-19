using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.CartItem
{
    public class CreateCartItemDTO
    {
        public int Quantity { get; set; }
        public int MealId { get; set; }
        public int CartId { get; set; }
    }
}
