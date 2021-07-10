using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Divisions.Repositories;

namespace CribblyBackend.Core.Divisions.Services
{
    public interface IDivisionService
    {
        Task<Division> GetById(int Id);
        void Update(Division division);
        Task<Division> Create(Division division);
        Task<Division> AddTeam(int id, Team team);
        void Delete(Division division);
    }
    public class DivisionService : IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;

        public DivisionService(IDivisionRepository divisionRepository)
        {
            _divisionRepository = divisionRepository;
        }

        public async Task<Division> GetById(int id)
        {
            return await _divisionRepository.GetById(id);
        }
        public async Task<Division> Create(Division division)
        {
            return await _divisionRepository.Create(division);
        }
        public async Task<Division> AddTeam(int id, Team team)
        {
            return await _divisionRepository.AddTeam(id, team);
        }
        public void Update(Division division)
        {
            throw new System.NotImplementedException();
        }
        public void Delete(Division division)
        {
            throw new System.NotImplementedException();
        }
    }
}