using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.DTOS.Review;

namespace Restaurant.Application.Services.ReviewSR
{
    public interface IReviewService
    {
        Task<List<AllReviewDTO>> GetAllReviews();
        Task<AllReviewDTO> GetReviewById(int reviewId);
        Task<List<AllReviewDTO>> GetAllReviewforUser(int customerId); 
        Task<AllReviewDTO> CreateReview(CreateReviewDTO dto, int userid);
        Task<AllReviewDTO> UpdateReview(CreateReviewDTO dto, int userid,int reviewid);
        Task<bool> DeleteReview(int reviewId, int userid);


    }
}
