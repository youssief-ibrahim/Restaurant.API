using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOS.Cart;
using Restaurant.Application.DTOS.CartItem;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.CartSR
{
    public class CartServices : ICartServices
    {
        private readonly IGenericRebosatory<Cart> GenCart;
        private readonly IGenericRebosatory<Meal> GenMeal;
        private readonly IGenericRebosatory<Customer> GenCustomer;
        private readonly IGenericRebosatory<CartItem> GenCartItem;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        public CartServices(IGenericRebosatory<Cart> GenCart, IGenericRebosatory<Meal> GenMeal, IGenericRebosatory<CartItem> GenCartItem, IGenericRebosatory<Customer> GenCustomer, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.GenCart = GenCart;
            this.GenMeal = GenMeal;
            this.GenCartItem = GenCartItem;
            this.GenCustomer = GenCustomer;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        public async Task<List<AllCartDTO>> GetAllCartsAsync()
        {
            var res = await GenCart.GetQueryable(c => c.CartItems)
                .Include(c => c.CartItems)
                .ThenInclude(c => c.Meal)
                .Include(c => c.Customer)
                .ToListAsync();
            if(!res.Any())
                throw new NotFoundException("No Carts Found");
            var data = mapper.Map<List<AllCartDTO>>(res);
            return data;
        }

        public async Task<AllCartDTO> GetCartByIdAsync(int id)
        {
            var res=await GenCart.GetQueryable(c=>c.CartItems)
                .Include(c=>c.CartItems)
                .ThenInclude(c=>c.Meal)
                .Include(c=>c.Customer)
                .FirstOrDefaultAsync(c=>c.CartId==id);
            if(res == null)
                throw new NotFoundException($"Not Found Cart with this Id {id}");

            var data = mapper.Map<AllCartDTO>(res);
            return data;
        }
        public async Task<AllCartItemDTO> CreateCartAsync(int userid, int mealid, int quantity)
        {
            var customer= await GenCustomer.GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");

            //var cart = await GenCart.GetById(c => c.CustomerId == customer.CustomerId, c => c.CartItems);
            var cart = await GenCart
                 .GetQueryable(c => c.CartItems)
                 .Where(c => c.CustomerId == customer.CustomerId)
                 .Include(c => c.CartItems)
                 .ThenInclude(ci => ci.Meal)
                 .Include(c => c.Customer)
                 .FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customer.CustomerId,
                    CartItems = new List<CartItem>()
                };
               await GenCart.Create(cart);
               await unitOfWork.SaveChanges();
            }
            var meal = await GenMeal.GetById(m => m.MealId == mealid);
            if (meal == null)
                throw new Exception($"Not Found Meal with this Id {mealid}");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MealId == mealid);

            int existingQuntity = cartItem?.Quantity ?? 0;   //0
            int totalQuantity = existingQuntity + quantity;  //5

          if (totalQuantity > meal.Quantity) //13
            {
                int availableQuantity = meal.Quantity - existingQuntity;
                if(availableQuantity > 0) throw new BadRequestException($"You can only add {availableQuantity}");
                else throw new BadRequestException("Out Of Stouk");
            }

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                GenCartItem.update(cartItem);
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    MealId = mealid,
                    Quantity = quantity
                };
                cart.CartItems.Add(cartItem);
            }
            await  unitOfWork.SaveChanges();
            var data = mapper.Map<AllCartItemDTO>(cartItem);
            return data;

        }
        public async Task<AllCartItemDTO> UpdateCartAsync(int userid, int mealid, int quantity)
        {
            var customer =await GenCustomer.GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");

            var cart = await GenCart
              .GetQueryable(c => c.CartItems)
              .Where(c => c.CustomerId == customer.CustomerId)
              .Include(c => c.CartItems)
              .ThenInclude(ci => ci.Meal)
              .Include(c => c.Customer)
              .FirstOrDefaultAsync();

            if (cart == null)
                throw new NotFoundException($"Not Found Cart for this UserId {userid}");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MealId == mealid);
            if (cartItem == null)
                throw new NotFoundException($"Not Found Meal with this Id {mealid} in Cart");

            //var meal = await GenMeal.GetById(m => m.MealId == mealid);
            var meal = cartItem.Meal;

            if (quantity == 0)
            {
                GenCartItem.delete(cartItem); // Remove the item from the cart
               await unitOfWork.SaveChanges();
                throw new BadRequestException("Meal removed from cart as quantity is set to 0");

            }
            
                if (quantity > meal.Quantity)
                    throw new BadRequestException($"only {meal.Quantity} are available in stock");

                cartItem.Quantity = quantity;
                GenCartItem.update(cartItem);
             
             await  unitOfWork.SaveChanges();
               var data = mapper.Map<AllCartItemDTO>(cartItem);
               return data;

        }
        public async Task<AllCartDTO> GetCartSummaryAsync(int userid)
        {
           var customer =await GenCustomer.GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");
            var cart = await GenCart
                .GetQueryable(c => c.CartItems)
                .Where(c => c.CustomerId == customer.CustomerId)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Meal)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync();
            if (cart == null)
                throw new NotFoundException($"Not Found Cart for this UserId {userid}");

            var data = mapper.Map<AllCartDTO>(cart);
            return data;
        }

        public async Task<bool> RemoveMealAsync(int mealid, int userid)
        {
           var customer =await GenCustomer.GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");
            var cart = GenCart
                .GetQueryable(c => c.CartItems)
                .Where(c => c.CustomerId == customer.CustomerId)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Meal)
                .FirstOrDefaultAsync().Result;
            if (cart == null)
                throw new NotFoundException($"Not Found Cart for this UserId {userid}");
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MealId == mealid);
            if (cartItem == null)
                throw new NotFoundException($"Not Found Meal with this Id {mealid} in Cart");
            GenCartItem.delete(cartItem);
           await unitOfWork.SaveChanges();
            return true;
        }
        public async Task<bool> ClereCartAsync(int userid)
        {
            var customer = await GenCustomer.GetById(c => c.UserId == userid);
            if (customer == null)
                throw new NotFoundException($"Not Found Customer with this UserId {userid}");
            var cart = await GenCart
                .GetQueryable(c => c.CartItems)
                .Where(c => c.CustomerId == customer.CustomerId)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Meal)
                .FirstOrDefaultAsync();
            if (cart == null)
                throw new NotFoundException($"Not Found Cart for this UserId {userid}");

            foreach (var item in cart.CartItems)
            {
                GenCartItem.delete(item);
            }
           await unitOfWork.SaveChanges();
            return true;
        }
    }
}
