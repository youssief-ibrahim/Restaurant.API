using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Review;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.CustomerSR
{
    public class CustomerServices : ICustomerServices
    {
        //private readonly IGenericRebosatory<Customer> GenCustomer;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public CustomerServices( IUnitOfWork unitOfWork, IMapper mapper)
        {
            //this.GenCustomer = GenCustomer;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<List<AllCustomerDTO>> GetAllAsync()
        {
           //var res=await GenCustomer.GetAll();
           var res=await unitOfWork.Genunit<Customer>().GetAll();
            if(!res.Any())
                throw new NotFoundException("No Customers Found");
            var data = mapper.Map<List<AllCustomerDTO>>(res);
            return data;
        }
        public async Task<List<AllReviewDTO>> GetAllCustomerReviewAsync(int userid)
        {
            var customer=await unitOfWork.Genunit<Customer>().GetById(c => c.UserId == userid,c=>c.Reviews);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer for this UserId {userid}");
            var res = customer.Reviews;
            if (!res.Any())
                throw new NotFoundException("No Reviews Found for this Customer");
            var data = mapper.Map<List<AllReviewDTO>>(res);
            return data;
        }
        public async Task<AllCustomerDTO> GetbyidAsync(int id)
        {
           var res = await unitOfWork.Genunit<Customer>().GetById(s=>s.CustomerId==id);
            if(res == null)
                throw new NotFoundException($"Not Found Customer with this Id {id}");
            var data = mapper.Map<AllCustomerDTO>(res);
            return data;
        }
        public async Task<AllCustomerDTO> CreateAsync(CreateCustomerDTO dto, int userid)
        {
            var exist = await unitOfWork.Genunit<Customer>().GetById(s => s.UserId == userid);
            if (exist != null)
                throw new BadRequestException("Customer Already Exist for this User");
            var customer = mapper.Map<Customer>(dto);
            customer.UserId = userid;
            await unitOfWork.Genunit<Customer>().Create(customer);
             await unitOfWork.SaveChanges();
            var data = mapper.Map<AllCustomerDTO>(customer);
            return data;
        }
        public async Task<AllCustomerDTO> UpdateAsync(CreateCustomerDTO dto, int userid)
        {
            var customer = await unitOfWork.Genunit<Customer>().GetById(c => c.UserId == userid);
            mapper.Map(dto, customer);
            customer.UserId = userid;
            unitOfWork.Genunit<Customer>().update(customer);
            await unitOfWork.SaveChanges();
            var data = mapper.Map<AllCustomerDTO>(customer);
            return data;
        }
        public async Task<bool> DeleteAsync(int userid)
        {
            var customer = await unitOfWork.Genunit<Customer>().GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer for this UserId {userid}");
            unitOfWork.Genunit<Customer>().delete(customer);
            await unitOfWork.SaveChanges();
            return true;
        }

        
    }
}
