using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,          // when checkout is done
        Confirmed = 1,       // when payment is confirmed >> weebhook success
        OnTheWay = 2,        // when delivery is assigned
        Delivered = 3,       // when order is delivered
        Cancelled = 4        // when order is cancelled  >> weebhook failed or user cancel
    }
}
