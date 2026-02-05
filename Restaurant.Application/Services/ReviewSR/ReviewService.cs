using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Restaurant.Application.DTOS.Review;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.ReviewSR
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<List<AllReviewDTO>> GetAllReviews()
        {
            var res =await unitOfWork.Genunit<Review>().GetAll();
            if(!res.Any())
                throw new NotFoundException("No Reviews Found");
            var data = mapper.Map<List<AllReviewDTO>>(res);
            return data;
        }
        public async Task<AllReviewDTO> GetReviewById(int reviewId)
        {
           var res=await unitOfWork.Genunit<Review>().GetById(r => r.ReviewId == reviewId);
            if (res == null)
                throw new NotFoundException($"Not Found Review with this Id {reviewId}");
            var data = mapper.Map<AllReviewDTO>(res);
            return data;
        }
        public async Task<List<AllReviewDTO>> GetAllReviewforUser(int customerId)
        {
            var customerid=await unitOfWork.Genunit<Customer>().GetById(c => c.CustomerId == customerId);
            if (customerid == null)
                throw new NotFoundException($"Not Found Customer with this Id {customerId}");
            var res = await unitOfWork.Genunit<Review>().FindAll(r => r.CustomerId == customerId);
            if (!res.Any())
                throw new NotFoundException($"No Reviews Found for this Customer Id {customerId}");
            var data = mapper.Map<List<AllReviewDTO>>(res);
            return data;
        }
        public async Task<AllReviewDTO> CreateReview(CreateReviewDTO dto, int userid)
        {
            var customer = await unitOfWork.Genunit<Customer>().GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");
            var meal =await unitOfWork.Genunit<Meal>().GetById(m => m.MealId == dto.MealId);
            if (meal == null)
                throw new NotFoundException($"Not Found Meal with this Id {dto.MealId}");
            var review = mapper.Map<Review>(dto);
            review.CustomerId = customer.CustomerId;
            await unitOfWork.Genunit<Review>().Create(review);
            await unitOfWork.SaveChanges();
            var data = mapper.Map<AllReviewDTO>(review);
            return data;
        }
        public async Task<AllReviewDTO> UpdateReview(CreateReviewDTO dto, int userid, int reviewid)
        {
            var customer = await unitOfWork.Genunit<Customer>().GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");
            var meal = await unitOfWork.Genunit<Meal>().GetById(m => m.MealId == dto.MealId);
            if (meal == null)
                throw new NotFoundException($"Not Found Meal with this Id {dto.MealId}");
            var review = await unitOfWork.Genunit<Review>().GetById(r => r.ReviewId == reviewid && r.CustomerId == customer.CustomerId);
            if (review == null)
                throw new NotFoundException($"Not Found Review with this Id {reviewid} for this Customer Id {customer.CustomerId}");
            mapper.Map(dto, review);
            review.CustomerId = customer.CustomerId;
            unitOfWork.Genunit<Review>().update(review);
            await unitOfWork.SaveChanges();
            var data = mapper.Map<AllReviewDTO>(review);
            return data;
        }
        public async Task<bool> DeleteReview(int reviewId, int userid)
        {
            var customer =await unitOfWork.Genunit<Customer>().GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");
            var review = await unitOfWork.Genunit<Review>().GetById(r => r.ReviewId == reviewId && r.CustomerId == customer.CustomerId);
            if (review == null)
                throw new NotFoundException($"Not Found Review with this Id {reviewId} for this Customer Id {customer.CustomerId}");
            unitOfWork.Genunit<Review>().delete(review);
            await unitOfWork.SaveChanges();
            return true;
        }

    }
}
 