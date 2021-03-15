using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface IDivisionService
    {
        Task<Division> GetById(int id);
        Task<int> Create(Division division);
    }
    public class DivisionService : IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;

        public DivisionService(IDivisionRepository divisionRepository)
        {
            _divisionRepository = divisionRepository;
        }

        public async Task<int> Create(Division division)
        {
            return await _divisionRepository.Create(division);
        }

        public async Task<Division> GetById(int id)
        {
            return await _divisionRepository.GetById(id);
        }
    }
}