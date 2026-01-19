using AutoMapper;
using Restaurant.Application.DTOS.Chef;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Exceptions;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Services.ChefSR
{
    public class ChefServices : IChefServices
    {
        private readonly IGenericRebosatory<Chef> GenChef;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ChefServices(IGenericRebosatory<Chef> GenChef, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.GenChef = GenChef;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<List<AllChefDTO>> GetAllAsync()
        {
            var res = await GenChef.GetAll();
            if (!res.Any())
                throw new NotFoundException("No Chefs Found");
            var data = mapper.Map<List<AllChefDTO>>(res);
            return data;
        }
        public async Task<AllChefDTO> GetbyidAsync(int id)
        {
            var res = await GenChef.GetById(s => s.ChefId == id);
            if (res == null)
                throw new NotFoundException($"Not Found Chef with this Id {id}");
            var data = mapper.Map<AllChefDTO>(res);
            return data;
        }
        public async Task<AllChefDTO> CreateAsync(CreateChefDTO dto, int userid)
        {
            var chef = mapper.Map<Chef>(dto);
            chef.UserId = userid;
            await GenChef.Create(chef);
            await unitOfWork.SaveChanges();
            var data = mapper.Map<AllChefDTO>(chef);
            return data;
        }
        public async Task<AllChefDTO> UpdateAsync(CreateChefDTO dto, int userid)
        {
            var chef = await GenChef.GetById(s => s.UserId == userid);
            if (chef == null)
                throw new NotFoundException($"Not Found Chef with this UserId {userid}");
            mapper.Map(dto, chef);
            chef.UserId = userid;
            GenChef.update(chef);
            await unitOfWork.SaveChanges();
            var data = mapper.Map<AllChefDTO>(chef);
            return data;
        }
        public async Task<bool> DeleteAsync(int userid)
        {
            var chef = await GenChef.GetById(s => s.UserId == userid);
            if (chef == null)
                throw new NotFoundException($"Not Found Chef with this UserId {userid}");
            GenChef.delete(chef);
            await unitOfWork.SaveChanges();
            return true;
        }
    }
}
