using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.CartItem
{
    public class AllCartItemDTO
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public int MealId { get; set; }
        public int CartId { get; set; }

        public string MealName { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
