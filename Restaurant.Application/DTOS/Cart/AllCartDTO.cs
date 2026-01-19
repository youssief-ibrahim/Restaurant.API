using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.CartItem;

namespace Restaurant.Application.DTOS.Cart
{
    public class AllCartDTO
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<AllCartItemDTO> Items { get; set; }= new List<AllCartItemDTO>();
        public int TotalQuantity => Items.Sum(i => i.Quantity);
        public decimal TotalPrice => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
