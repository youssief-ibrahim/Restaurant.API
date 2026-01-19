using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Restaurant.Application.DTOS.Cart;
using Restaurant.Application.DTOS.CartItem;
using Restaurant.Application.DTOS.Chef;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Delivery;
using Restaurant.Application.DTOS.Meal;
using Restaurant.Application.DTOS.Order;
using Restaurant.Application.DTOS.OrderItem;
using Restaurant.Application.DTOS.Review;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Common.Mapping
{
    public class HelperMapper: Profile
    {
        public HelperMapper()
        {
            // Customer 
            CreateMap<Customer,AllCustomerDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)).ReverseMap();
            CreateMap<CreateCustomerDTO, Customer>().ReverseMap();

            // Chef
            CreateMap<Chef, AllChefDTO>()
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)).ReverseMap();
            CreateMap<CreateChefDTO, Chef>().ReverseMap();

            // Delivery
            CreateMap<Delivery, AllDeliveryDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)).ReverseMap();
            CreateMap<CreateDeliveryDTO, Delivery>().ReverseMap();

            // order
            CreateMap<Order,AllOrderDTO>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId)).ReverseMap();
            CreateMap<CreateOrderDTO, Order>().ReverseMap();
            CreateMap<Order,OrderStatusDTO>().ReverseMap();

            // orderitem
            CreateMap<OrderItem, AllOrderItemDTO>()
                //.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Meal.Price))
                .ReverseMap();
            CreateMap<CreateOrderItemDTO, OrderItem>().ReverseMap();

            // meal
            CreateMap<Meal, AllMealDTO>()
            .ForMember(dest => dest.ChefId, opt => opt.MapFrom(src => src.ChefId)).ReverseMap();
            CreateMap<CreateMealDTO, Meal>().ReverseMap();
            // review
            CreateMap<Review, AllReviewDTO>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId)).ReverseMap();
            CreateMap<CreateReviewDTO, Review>().ReverseMap();

            // cartitem
            CreateMap<CartItem,AllCartItemDTO>()
            .ForMember(dest => dest.MealName, opt => opt.MapFrom(src => src.Meal.Name))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Meal.Price))
            .ReverseMap();

            // cart
            CreateMap<Cart, AllCartDTO>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ReverseMap();

        }
    }
}
