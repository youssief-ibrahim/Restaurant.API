using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOS.Delivery;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.DeliverySR
{
    public class DeliveryServices : IDeliveryServices
    {
        //private readonly IGenericRebosatory<Delivery> GenDelivary;
        //private readonly IGenericRebosatory<Order> GenOrder;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public DeliveryServices( /*IGenericRebosatory<Order> GenOrder*/ IUnitOfWork unitOfWork, IMapper mapper)
        {
            //this.GenDelivary = GenDelivary;
            //this.GenOrder = GenOrder;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<AllDeliveryDTO> CreateAsync(int userid, int orderid, string address)
        {
            var order = await unitOfWork.Genunit<Order>().GetById(o => o.OrderId == orderid);
            if (order == null)
                throw new NotFoundException($"Not Found Order with this Id {orderid}");
            var existingDelivery = await unitOfWork.Genunit<Delivery>().GetById(d => d.OrderId == orderid);
            if (existingDelivery != null)
                throw new BadRequestException($"Delivery already assigned for OrderId {orderid}");

            var delivery = new Delivery
            {
                OrderId = orderid,
                UserId = userid,
                Address = address,
                Status = DeliveryStatus.Pending
            };
            try
            {
                await unitOfWork.Genunit<Delivery>().Create(delivery);
                order.Status = OrderStatus.OnTheWay;
                unitOfWork.Genunit<Order>().update(order);
                await unitOfWork.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                
                throw new BadRequestException($"Can not assign more order to this Dilevary cause he assigned to another order");
            }

            var data = mapper.Map<AllDeliveryDTO>(delivery);
            return data;
        }

        public async Task<bool> CancleAsync(int userid,int orderid)
        {
            var delivery = await unitOfWork.Genunit<Delivery>().GetById(d => d.OrderId == orderid && d.UserId == userid);
            if (delivery == null)
                throw new NotFoundException($"Not Found Delivery for this UserId {userid}");

            var order = await unitOfWork.Genunit<Order>().GetById(o => o.OrderId == orderid);
            if (order != null)
            {
                order.Status = OrderStatus.Cancelled;
                unitOfWork.Genunit<Order>().update(order);
                unitOfWork.Genunit<Delivery>().delete(delivery);
                await unitOfWork.SaveChanges();
            }
            else throw new NotFoundException($"Not Found Order with this Id {orderid}");

            return true;
        }

        public async Task<List<AllDeliveryDTO>> GetAllAsync()
        {
            var res=await unitOfWork.Genunit<Delivery>().GetAll();
            if(!res.Any())
                throw new NotFoundException("No Deliveries Found");
            var data = mapper.Map<List<AllDeliveryDTO>>(res);
            return data;
        }

        public async Task<AllDeliveryDTO> GetByIdAsync(int id)
        {
            var res= await unitOfWork.Genunit<Delivery>().GetById(d => d.DeliveryId == id);
            if (res == null)
                throw new NotFoundException($"Not Found Delivery with this Id {id}");
            var data = mapper.Map<AllDeliveryDTO>(res);
            return data;
        }

        public async Task<AllDeliveryDTO> GetByOrderidAsync(int orderid)
        {
           var res =await unitOfWork.Genunit<Delivery>().GetById(d => d.OrderId == orderid);
            if (res == null)
                throw new NotFoundException($"Not Found Delivery for this OrderId {orderid}");
            var data = mapper.Map<AllDeliveryDTO>(res);
            return data;
        }

        public async Task<AllDeliveryDTO> UpdateAsync(int userid, int orderid, DeliveryStatus status)
        {
            var delivery =await unitOfWork.Genunit<Delivery>().GetById(d=>d.OrderId==orderid && d.UserId==userid);
            if (delivery == null)
                throw new NotFoundException($"Not Found Delivery for this UserId {userid}");

           delivery.Status = status;
            if(status == DeliveryStatus.Delivered)
            {
                var order = await unitOfWork.Genunit<Order>().GetById(o => o.OrderId == orderid);
                if (order != null)
                {
                    order.Status = OrderStatus.Delivered;
                    unitOfWork.Genunit<Order>().update(order);
                }
            }

            unitOfWork.Genunit<Delivery>().update(delivery);
            await unitOfWork.SaveChanges();
            var data = mapper.Map<AllDeliveryDTO>(delivery);
            return data;
        }
    }
}
