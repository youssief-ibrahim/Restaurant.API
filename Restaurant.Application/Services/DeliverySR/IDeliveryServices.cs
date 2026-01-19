using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Delivery;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services.DeliverySR
{
    public interface IDeliveryServices
    {
        Task<List<AllDeliveryDTO>> GetAllAsync();
        Task<AllDeliveryDTO> GetByIdAsync(int id);
        Task<AllDeliveryDTO> GetByOrderidAsync(int orderid);
        Task<AllDeliveryDTO> CreateAsync(int userid,int orderid,string address );
        Task<AllDeliveryDTO> UpdateAsync(int userid, int orderid, DeliveryStatus status);
        Task<bool> CancleAsync(int userid, int orderid);

    }
}
